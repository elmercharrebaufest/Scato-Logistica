using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Molinos.Scato.Servicios;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile
{
    public partial class Startup
    {
        private readonly string ClientId =
            ConfigurationManager.AppSettings["ida:ClientId"];

        private readonly string clientSecret =
            ConfigurationManager.AppSettings["ida:ClientSecret"];

        private readonly string Authority =
            ConfigurationManager.AppSettings["ida:Authority"];

        private readonly string RedirectUri = 
            ConfigurationManager.AppSettings["ida:RedirectUri"];

        private readonly string PostLogoutRedirectUri =
            ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        private readonly bool UseHttpsConfig =
            bool.Parse(ConfigurationManager.AppSettings["ida:UseHttps"] ?? "true");

        private readonly int CookieExpireTimeHours =
            int.Parse(ConfigurationManager.AppSettings["ida:CookieExpireTimeHours"] ?? "8");

        private ILogger _log;

        public void ConfigureAuth(IAppBuilder app)
        {
            var loggerFactory = DependencyResolver.Current.GetService<ILoggerFactory>();
            _log = loggerFactory?.GetCurrentClassLogger();

            // =============================
            // Cookie Authentication
            // =============================
            app.SetDefaultSignInAsAuthenticationType(
                CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromHours(CookieExpireTimeHours),
                CookieManager = new Microsoft.Owin.Host.SystemWeb.SystemWebChunkingCookieManager(),
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType
            });

            // =============================
            // OpenID Connect (Azure Entra ID)
            // =============================
            var optionsOpenIdAuth = new OpenIdConnectAuthenticationOptions
            {
                ClientId = ClientId,
                ClientSecret = clientSecret,
                Authority = Authority,

                RedirectUri = RedirectUri,
                PostLogoutRedirectUri = PostLogoutRedirectUri,

                ResponseType = OpenIdConnectResponseType.Code,
                RedeemCode = true,
                Scope = "openid profile email User.Read", //ver si hace falta agregar offline_access
                SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,

                CookieManager = new Microsoft.Owin.Host.SystemWeb.SystemWebCookieManager(),

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = OnAuthenticationFailed,
                    SecurityTokenValidated = OnSecurityTokenValidated,
                    AuthorizationCodeReceived = OnAuthorizationCodeReceived
                }
            };

            if (optionsOpenIdAuth.ProtocolValidator != null)
            {
                // En true funciona solo con https. En false funciona con ambos pero el default es true.
                optionsOpenIdAuth.ProtocolValidator.RequireNonce = UseHttpsConfig;
            }

            app.UseOpenIdConnectAuthentication(optionsOpenIdAuth);
        }

        // =============================
        // Event handlers
        // =============================

        /// <summary>
        /// Handles when authorization code is received from Entra ID
        /// </summary>
        private Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification context)
        {
            var loggerFactory = DependencyResolver.Current.GetService<ILoggerFactory>();
            _log = loggerFactory?.GetCurrentClassLogger();
            _log?.Info("Authorization code recibido de Entra ID. RedirectUri usado: {0}", context.RedirectUri);
            
            return Task.CompletedTask;
        }

        private Task OnAuthenticationFailed(
            AuthenticationFailedNotification<
                OpenIdConnectMessage,
                OpenIdConnectAuthenticationOptions> context)
        {
            var loggerFactory = DependencyResolver.Current.GetService<ILoggerFactory>();
            _log = loggerFactory?.GetCurrentClassLogger();

            _log?.Error(context.Exception, "Error de autenticación Entra ID");

            context.HandleResponse();
            
            // Use VirtualPathUtility to construct path relative to the application base
            var pathBase = context.Request.PathBase.Value ?? "";
            var errorUrl = $"{pathBase}/ErrorPages/GenericError.html?message=" +
                Uri.EscapeDataString(context.Exception.Message);
            
            context.Response.Redirect(errorUrl);

            return Task.CompletedTask;
        }

        private async Task OnSecurityTokenValidated(
            SecurityTokenValidatedNotification<
                OpenIdConnectMessage,
                OpenIdConnectAuthenticationOptions> context)
        {
            var identity = context.AuthenticationTicket.Identity;

            var loggerFactory = DependencyResolver.Current.GetService<ILoggerFactory>();
            _log = loggerFactory?.GetCurrentClassLogger();

            var username = await ExtractUsername(identity, context);

            if (string.IsNullOrEmpty(username))
            {
                _log?.Warn("No se pudo determinar el usuario desde los claims ni desde Graph API");
                return;
            }

            _log?.Info("Usuario autenticado: {0}", username);

            LoadUserPermissionsAndClaims(identity, username);

            // Check if user has any roles
            var hasRoles = identity.Claims.Any(c => c.Type == ClaimTypes.Role);
            if (!hasRoles)
            {
                _log?.Warn("Usuario {0} no tiene roles asignados en la aplicación", username);
                
                context.HandleResponse();
                
                var pathBase = context.Request.PathBase.Value ?? "";
                var errorUrl = $"{pathBase}/ErrorPages/GenericError.html?message=" +
                    Uri.EscapeDataString("El usuario no tiene permisos asignados para esta aplicación. Por favor, contacte a soporte.");
                
                context.Response.Redirect(errorUrl);
                return;
            }

            // Update the authentication ticket with the modified identity
            context.AuthenticationTicket = new Microsoft.Owin.Security.AuthenticationTicket(
                identity,
                context.AuthenticationTicket.Properties);

            return;
        }

        // =============================
        // Helper Methods
        // =============================

        /// <summary>
        /// Extracts username from claims or Graph API
        /// Priority: 1) onprem_sam_account_name claims (various formats), 2) Graph API, 3) preferred_username (parsed), 4) UPN/Name
        /// </summary>
        private async Task<string> ExtractUsername(
            ClaimsIdentity identity, 
            SecurityTokenValidatedNotification<OpenIdConnectMessage,
            OpenIdConnectAuthenticationOptions> context
            )
        {
            // 1. Intentar onprem_sam_account_name desde claims (usuarios sincronizados desde AD on-prem)
            // Probar múltiples formatos de claim conocidos para SAM account name
            var samAccountClaimTypes = new[]
            {
                "onprem_samaccountname", //este es el que se esta usando actualmente
                "onprem_sam_account_name",
            };

            foreach (var claimType in samAccountClaimTypes)
            {
                var username = identity.FindFirst(claimType)?.Value;
                if (!string.IsNullOrEmpty(username))
                {
                    // Si el claim contiene DOMAIN\username, extraer solo el username
                    if (username.Contains("\\"))
                    {
                        username = username.Substring(username.LastIndexOf('\\') + 1);
                    }

                    _log?.Info($"Username obtenido desde claim '{claimType}': {username}");
                    return username;
                }
            }

            // 2. Si no existe en claims, intentar obtenerlo desde Microsoft Graph API
            try
            {
                var username = await GetUsernameFromGraphApi(context);
                if (!string.IsNullOrEmpty(username))
                {
                    _log?.Info($"Username obtenido desde Microsoft Graph API: {username}");
                    return username;
                }
            }
            catch (Exception ex)
            {
                _log?.Error($"Error al intentar obtener username desde Graph API: {ex.Message}");
            }

            // 3. Fallback: usar preferred_username y extraer la parte antes del @
            var preferredUsername = identity.FindFirst("preferred_username")?.Value;
            if (!string.IsNullOrEmpty(preferredUsername))
            {
                // Extraer "Fernando.Pagano" de "Fernando.Pagano@molinosagro.com.ar"
                var atIndex = preferredUsername.IndexOf('@');
                var username = atIndex > 0 ? preferredUsername.Substring(0, atIndex) : preferredUsername;
                _log?.Info($"Username obtenido desde preferred_username (parseado): {username}");
                return username;
            }

            // 4. Último fallback: UPN o Name
            var finalUsername = identity.FindFirst(ClaimTypes.Upn)?.Value ??
                       identity.FindFirst(ClaimTypes.Name)?.Value;

            _log?.Info($"Username obtenido desde UPN o Name: {finalUsername}");

            return finalUsername;
        }

        /// <summary>
        /// Fetches the on-premises SAM account name from Microsoft Graph API
        /// </summary>
        private async Task<string> GetUsernameFromGraphApi(
            SecurityTokenValidatedNotification<OpenIdConnectMessage,
            OpenIdConnectAuthenticationOptions> context
            )
        {
            var identity = context.AuthenticationTicket.Identity;
            
            // Obtener el Object ID del usuario desde los claims
            var objectId = identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            if (string.IsNullOrEmpty(objectId))
            {
                _log?.Error("No se encontró el ObjectId en los claims. No se puede consultar Graph API.");
                return null;
            }

            // Obtener el access token para Microsoft Graph
            var accessToken = await GetGraphAccessToken(context);
            if (string.IsNullOrEmpty(accessToken))
            {
                _log?.Error("No se pudo obtener el access token para Microsoft Graph API");
                return null;
            }

            // Llamar a Microsoft Graph API para obtener el perfil del usuario
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", accessToken);
                
                // Solicitar solo el campo onPremisesSamAccountName para optimizar la consulta
                var graphUrl = $"https://graph.microsoft.com/v1.0/users/{objectId}?$select=onPremisesSamAccountName,userPrincipalName,displayName";

                _log?.Info($"Consultando Microsoft Graph API: {graphUrl}");

                var response = await httpClient.GetAsync(graphUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var user = JObject.Parse(json);
                    
                    var samAccountName = user["onPremisesSamAccountName"]?.ToString();
                    
                    if (!string.IsNullOrEmpty(samAccountName))
                    {
                        _log?.Info($"onPremisesSamAccountName obtenido desde Graph API: {samAccountName}");
                        return samAccountName;
                    }
                    else
                    {
                        _log?.Error("El usuario no tiene onPremisesSamAccountName en Graph API (posiblemente usuario cloud-only o invitado)");
                    }
                }
                else
                {
                    _log.Error("Error al consultar Graph API. Status: {0}, Reason: {1}", response.StatusCode, response.ReasonPhrase);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets an access token for Microsoft Graph API
        /// For RedeemCode=true: Uses the already-redeemed access token from ProtocolMessage
        /// For accesstoken=null: Exchanges the authorization code for a new token
        /// </summary>
        private async Task<string> GetGraphAccessToken(
            SecurityTokenValidatedNotification<OpenIdConnectMessage,
                OpenIdConnectAuthenticationOptions> context
            )
        {
            try
            {
                // 1. Si RedeemCode=true, el access token ya está disponible
                var accessToken = context.ProtocolMessage?.AccessToken;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    // Verificar que el token tiene el scope necesario para Graph API
                    var scope = context.ProtocolMessage.Parameters?.ContainsKey("scope") == true 
                        ? context.ProtocolMessage.Parameters["scope"] 
                        : string.Empty;
                    
                    if (scope.Contains("User.Read"))
                    {
                        _log?.Info($"Access token para Graph API obtenido desde ProtocolMessage (RedeemCode=true). Scope: {scope}");
                        return accessToken;
                    }
                    else
                    {
                        _log?.Info($"Access token disponible pero sin scope User.Read. Scope actual: {scope}");
                    }
                }

                // 2. Fallback: Obtener el código de autorización y canjearlo manualmente (caso HTTPS)
                var code = context.ProtocolMessage?.Code;
                
                if (string.IsNullOrEmpty(code))
                {
                    _log?.Error("No se encontró ni access token ni authorization code en el contexto");
                    return null;
                }

                _log?.Info("Canjeando authorization code por access token para Graph API (caso HTTPS)");

                using (var httpClient = new HttpClient())
                {
                    var tokenEndpoint = $"{Authority.Replace("/v2.0", "")}/oauth2/v2.0/token";
                    
                    var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
                    {
                        Content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("client_id", ClientId),
                            new KeyValuePair<string, string>("client_secret", clientSecret),
                            new KeyValuePair<string, string>("code", code),
                            new KeyValuePair<string, string>("redirect_uri", RedirectUri),
                            new KeyValuePair<string, string>("grant_type", "authorization_code"),
                            new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/User.Read")
                        })
                    };

                    var response = await httpClient.SendAsync(tokenRequest);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JObject.Parse(json);
                        var newAccessToken = tokenResponse["access_token"]?.ToString();

                        _log?.Info($"Access token para Graph API obtenido mediante canje de código");
                        return newAccessToken;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _log?.Error($"Error al obtener access token para Graph. Status: {response.StatusCode}, Error: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.Error($"Excepción al intentar obtener access token para Graph API: {ex}");
            }

            return null;
        }

        /// <summary>
        /// Loads user permissions and additional claims (centro, roles, etc.)
        /// </summary>
        private void LoadUserPermissionsAndClaims(ClaimsIdentity identity, string username)
        {
            var repo = DependencyResolver.Current.GetService<IServicioRepositorio>();
            var config = DependencyResolver.Current.GetService<IConfiguracionProvider>();

            if (repo == null || config == null)
            {
                _log?.Error("Servicios de dominio no disponibles para cargar claims");
                return;
            }

            // Roles / permisos
            var permisos = repo.ListarPermisosPorUsuario(username);
            foreach (var permiso in permisos)
            {
                if (permiso?.Codigo != null)
                {
                    identity.AddClaim(
                        new Claim(ClaimTypes.Role, permiso.Codigo.Value.ToString()));
                }
            }

            // Claims de centro
            var centro = repo.ObtenerCentroPorCodigoSap(config.AppSettings["CodigoSapSanLorenzo"]);

            if (centro != null)
            {
                identity.AddClaim(new Claim("CentroDescripcion", centro.Descripcion ?? ""));
                identity.AddClaim(new Claim("CentroId", centro.Id.ToString(CultureInfo.InvariantCulture)));
                identity.AddClaim(new Claim("EstacionMeteorologica", centro.CodigoEstacionMeteorologica ?? "noConfigurada"));
            }

            _log?.Info($"Claims cargados correctamente. Total: {identity.Claims.Count()}");
        }
    }

}

