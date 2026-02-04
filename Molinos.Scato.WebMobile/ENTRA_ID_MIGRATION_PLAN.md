# Guía de Migración a Azure Entra ID - Molinos.Scato.WebMobile

## Resumen General
Este documento describe la migración desde ADFS (Active Directory Federation Services) hacia Azure Entra ID (anteriormente Azure Active Directory) para la aplicación Molinos.Scato.WebMobile.

**Estado de la Migración:** ?? **EN PROGRESO** - Implementación de código completa, fase de pruebas

### Aspectos Destacados de la Implementación
- ? **Middleware OWIN**: Autenticación OpenID Connect usando `Microsoft.Owin.Security.OpenIdConnect` 4.2.3
- ? **Autenticación Híbrida**: Soporta tanto usuarios sincronizados desde AD on-premises como usuarios solo en la nube
- ? **Integración con Graph API**: Fallback a Microsoft Graph API para recuperar `onPremisesSamAccountName` cuando no está en los claims
- ? **Flujo de Código de Autorización**: Usa `ResponseType = Code` con `RedeemCode = true` para mayor seguridad
- ? **Transformación de Claims**: Lógica personalizada en `OnSecurityTokenValidated` para cargar permisos desde base de datos
- ? **Módulo de Desarrollo**: `DummyAuthenticationModule` para desarrollo local sin Entra ID
- ? **Gestión de Sesión**: Expiración deslizante de 8 horas con soporte de cookies fragmentadas para conjuntos grandes de claims
- ? **Manejo de Errores**: Manejo personalizado de errores con redirecciones a página de error genérica

---

## Estado Actual

### Configuración (Web.config)
La aplicación actualmente usa WS-Federation con ADFS:

**Configuración ADFS:**
- **ida:FederationMetadataLocation**: `https://bfdev271/FederationMetadata/2007-06/FederationMetadata.xml`
- **ida:Issuer**: `https://bfdev271.baunet.local/adfs/ls/`
- **ida:ProviderSelection**: `productionSTS`
- **UrlAdfsLogoff**: `https://bfdev271.baunet.local/adfs/ls/?wa=wsignout1.0`

### Componentes de Autenticación

#### 1. FixedWsFederationAuthenticationModule
- **Ubicación**: `Molinos.Scato.WebMobile\Seguridad\FixedWsFederationAuthenticationModule.cs`
- **Propósito**: Módulo WS-Federation personalizado que corrige el bug de barra final en URLs de retorno
- **Estado**: Activo en producción, comentado en desarrollo

#### 2. DummyAuthenticationModule
- **Ubicación**: `Molinos.Scato.WebMobile\Seguridad\DummyAuthenticationModule.cs`
- **Propósito**: Módulo solo para desarrollo que omite ADFS
- **Funcionalidad**: 
  - Crea claims falsos para el usuario `molinosagro\lavrench`
  - Asigna todos los permisos del enum `PermisosScato`
  - Establece centro predeterminado: SLO (CentroId: 5)

#### 3. ScatoClaimsAuthenticationManager
- **Ubicación**: `Molinos.Scato.WebMobile\Seguridad\ScatoClaimsAuthenticationManager.cs`
- **Propósito**: Transformación de claims post-autenticación
- **Funcionalidad**:
  - Extrae nombre de usuario desde `ClaimTypes.NameIdentifier`
  - Carga permisos de usuario desde base de datos vía `ServicioRepositorio.ListarPermisosPorUsuario()`
  - Agrega claims de rol para cada permiso
  - Agrega claims específicos del centro (CentroDescripcion, CentroId, EstacionMeteorologica)

#### 4. ChunkedSecurityCookieHandler
- **Ubicación**: `Molinos.Scato.WebMobile\Seguridad\ChunkedSecurityCookieHandler.cs`
- **Propósito**: Manejador de cookies personalizado para cookies de autenticación grandes
- **Uso**: Configurado en `system.identityModel.services/federationConfiguration/cookieHandler`

#### 5. SessionAuthenticationModule
- Módulo estándar WIF para gestión de sesión
- Usa MachineKeySessionSecurityTokenHandler para encriptación de cookies

### Configuración de Seguridad en Web.config

```xml
<system.identityModel>
  - Validación de certificado: None (desarrollo)
  - Administrador de autenticación de claims: ScatoClaimsAuthenticationManager
  - Emisor: BFDEV271.baunet.local/adfs/services/trust
  - Manejadores de tokens: MachineKeySessionSecurityTokenHandler
</system.identityModel>

<system.identityModel.services>
  - Manejador de cookies: ChunkedSecurityCookieHandler personalizado
  - Duración de sesión: 1 hora (persistentSessionLifetime="1:0:0")
  - Configuración WS-Federation
</system.identityModel.services>
```

---

## Estado Objetivo (Azure Entra ID)

### Nuevas Claves de Configuración (Agregadas a Web.config)

```xml
<!-- Configuración Azure Entra ID (Azure AD) -->
<add key="ida:TenantId" value="790c9737-0b8e-4138-a0f4-819cdc1eb64b" />
<add key="ida:ClientId" value="08b409d6-e101-4b65-8088-2bfd770baaa7" />
<add key="ida:ClientSecret" value="xxxx" />
<add key="ida:RedirectUri" value="http://localhost/Scato.WebMobile/signin-oidc" />
<add key="ida:PostLogoutRedirectUri" value="http://localhost/Scato.WebMobile/" />
<add key="ida:Authority" value="https://login.microsoftonline.com/790c9737-0b8e-4138-a0f4-819cdc1eb64b/v2.0" />
<add key="ida:UseHttps" value="false" />
<add key="owin:AppStartup" value="Molinos.Scato.WebMobile.Startup" />
```

**Estado:** ? **COMPLETADO** - Valores de configuración de desarrollo establecidos

### Configuración de Registro de Aplicación en Azure

#### Configuraciones Establecidas en el Portal de Azure:
1. **ID de Aplicación (cliente)**: `08b409d6-e101-4b65-8088-2bfd770baaa7` ?
2. **ID de Directorio (inquilino)**: `790c9737-0b8e-4138-a0f4-819cdc1eb64b` ?
3. **Secreto de Cliente**: Configurado (expira en fecha programada de rotación) ?
4. **URIs de Redirección**: ? Configurado
   - Desarrollo: `http://localhost/Scato.WebMobile/signin-oidc`
   - ?? Las URLs de producción deben agregarse durante el despliegue
5. **Autenticación**: ? Configurado
   - Plataforma: Web
   - Tokens de ID: Habilitados
   - Tokens de acceso: Habilitados (para llamadas a Graph API)
6. **Permisos de API**: ? Configurado
   - Microsoft Graph: `User.Read` (delegado) - para acceder al perfil de usuario y `onPremisesSamAccountName`
7. **Configuración de Token** (Claims Opcionales): ? Configurado
   - `email`, `preferred_username`, `onprem_samaccountname` agregados

---

## Pasos de Migración

### Fase 1: Preparación y Paquetes NuGet

#### 1.1 Instalar Paquetes Requeridos
**Framework Objetivo**: .NET Framework 4.7.2
**Nota**: El middleware OWIN es el enfoque recomendado para aplicaciones .NET Framework 4.x

**Paquetes NuGet Instalados:**
```xml
<!-- Middleware OWIN -->
<package id="Microsoft.Owin" version="4.2.3" targetFramework="net472" />
<package id="Microsoft.Owin.Host.SystemWeb" version="4.2.3" targetFramework="net472" />
<package id="Microsoft.Owin.Security" version="4.2.3" targetFramework="net472" />
<package id="Microsoft.Owin.Security.Cookies" version="4.2.3" targetFramework="net472" />
<package id="Microsoft.Owin.Security.OpenIdConnect" version="4.2.3" targetFramework="net472" />
<package id="Owin" version="1.0" targetFramework="net472" />

<!-- Identity Model y JWT -->
<package id="Microsoft.IdentityModel.Abstractions" version="8.15.0" targetFramework="net472" />
<package id="Microsoft.IdentityModel.JsonWebTokens" version="8.15.0" targetFramework="net472" />
<package id="Microsoft.IdentityModel.Logging" version="8.15.0" targetFramework="net472" />
<package id="Microsoft.IdentityModel.Protocols" version="8.15.0" targetFramework="net472" />
<package id="Microsoft.IdentityModel.Protocols.OpenIdConnect" version="8.15.0" targetFramework="net472" />
<package id="Microsoft.IdentityModel.Tokens" version="8.15.0" targetFramework="net472" />
<package id="System.IdentityModel.Tokens.Jwt" version="8.15.0" targetFramework="net472" />

<!-- Cliente de Microsoft Identity (para Graph API si es necesario) -->
<package id="Microsoft.Identity.Client" version="4.81.0" targetFramework="net472" />

<!-- Librerías de Soporte -->
<package id="Newtonsoft.Json" version="13.0.1" targetFramework="net472" />
<package id="System.Memory" version="4.5.5" targetFramework="net472" />
<package id="System.Buffers" version="4.5.1" targetFramework="net472" />
<package id="System.Runtime.CompilerServices.Unsafe" version="6.0.0" targetFramework="net472" />
<package id="System.Text.Json" version="8.0.5" targetFramework="net472" />
<package id="System.Text.Encodings.Web" version="8.0.0" targetFramework="net472" />
```

#### 1.2 Actualizar Configuración
- ? **COMPLETADO**: Agregar claves de Entra ID a `Web.config` appSettings
- ?? **PENDIENTE**: Reemplazar valores de producción para:
  - `YOUR_TENANT_ID`
  - `YOUR_CLIENT_ID`
  - `YOUR_CLIENT_SECRET`

### Fase 2: Cambios de Código

#### 2.1 Crear Clase de Inicio OWIN
**Archivo**: `Molinos.Scato.WebMobile\Startup.cs` (raíz)

```csharp
using Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(Molinos.Scato.WebMobile.Startup))]

namespace Molinos.Scato.WebMobile
{
    /// <summary>
    /// Clase de inicio OWIN para configurar el middleware de autenticación
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Punto de entrada de configuración para el pipeline OWIN
        /// </summary>
        /// <param name="app">El constructor de aplicación OWIN</param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
```

**Archivo**: `Molinos.Scato.WebMobile\App_Start\Startup.Auth.cs`

```csharp
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

        private ILogger _log;

        public void ConfigureAuth(IAppBuilder app)
        {
            var loggerFactory = DependencyResolver.Current.GetService<ILoggerFactory>();
            _log = loggerFactory?.GetCurrentClassLogger();

            // =============================
            // Autenticación por Cookie
            // =============================
            app.SetDefaultSignInAsAuthenticationType(
                CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromHours(8),
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
                Scope = "openid profile email User.Read",
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
                // En true funciona solo con https. En false funciona con ambos pero no es la mejor configuración.
                optionsOpenIdAuth.ProtocolValidator.RequireNonce = UseHttpsConfig;
            }

            app.UseOpenIdConnectAuthentication(optionsOpenIdAuth);
        }

        // =============================
        // Manejadores de Eventos
        // =============================

        /// <summary>
        /// Maneja cuando se recibe el código de autorización desde Entra ID
        /// </summary>
        private Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification context)
        {
            _log?.Info("Authorization code recibido de Entra ID. RedirectUri usado: {0}", context.RedirectUri);
            return Task.CompletedTask;
        }

        private Task OnAuthenticationFailed(
            AuthenticationFailedNotification<
                OpenIdConnectMessage,
                OpenIdConnectAuthenticationOptions> context)
        {
            _log?.Error(context.Exception, "Error de autenticación Entra ID");

            context.HandleResponse();
            
            // Usar VirtualPathUtility para construir ruta relativa a la base de la aplicación
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

            var username = await ExtractUsername(identity, context);

            if (string.IsNullOrEmpty(username))
            {
                _log?.Warn("No se pudo determinar el usuario desde los claims ni desde Graph API");
                return;
            }

            _log?.Info("Usuario autenticado: {0}", username);

            LoadUserPermissionsAndClaims(identity, username);

            // Verificar si el usuario tiene algún rol
            var hasRoles = identity.Claims.Any(c => c.Type == ClaimTypes.Role);
            if (!hasRoles)
            {
                _log?.Warn("Usuario {0} no tiene roles asignados en la aplicación", username);
                
                context.HandleResponse();
                
                var pathBase = context.Request.PathBase.Value ?? "";
                var errorUrl = $"{pathBase}/ErrorPages/GenericError.html?message=" +
                    Uri.EscapeDataString("El usuario no tiene permisos asignados para esta aplicación. Por favor, contacte al administrador.");
                
                context.Response.Redirect(errorUrl);
                return;
            }

            // Actualizar el ticket de autenticación con la identidad modificada
            context.AuthenticationTicket = new Microsoft.Owin.Security.AuthenticationTicket(
                identity,
                context.AuthenticationTicket.Properties);
        }

        // =============================
        // Métodos Auxiliares
        // =============================

        /// <summary>
        /// Extrae el nombre de usuario desde claims o Graph API
        /// Prioridad: 1) claims onprem_sam_account_name (varios formatos), 2) Graph API, 3) preferred_username (parseado), 4) UPN/Name
        /// </summary>
        private async Task<string> ExtractUsername(
            ClaimsIdentity identity, 
            SecurityTokenValidatedNotification<OpenIdConnectMessage,
            OpenIdConnectAuthenticationOptions> context)
        {
            // 1. Intentar onprem_sam_account_name desde claims (usuarios sincronizados desde AD on-prem)
            var samAccountClaimTypes = new[]
            {
                "onprem_samaccountname",
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
        /// Obtiene el nombre de cuenta SAM on-premises desde Microsoft Graph API
        /// </summary>
        private async Task<string> GetUsernameFromGraphApi(
            SecurityTokenValidatedNotification<OpenIdConnectMessage,
            OpenIdConnectAuthenticationOptions> context)
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
        /// Obtiene un token de acceso para Microsoft Graph API
        /// Para RedeemCode=true: Usa el access token ya canjeado desde ProtocolMessage
        /// Para accesstoken=null: Intercambia el código de autorización por un nuevo token
        /// </summary>
        private async Task<string> GetGraphAccessToken(
            SecurityTokenValidatedNotification<OpenIdConnectMessage,
                OpenIdConnectAuthenticationOptions> context)
        {
            try
            {
                // 1. Si RedeemCode=true, el access token ya está disponible
                var accessToken = context.ProtocolMessage?.AccessToken;
                if (!string.IsNullOrEmpty(accessToken))
                {
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
        /// Carga permisos de usuario y claims adicionales (centro, roles, etc.)
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
```

#### 2.2 Actualizar Web.config

**Configuración de Azure Entra ID:**
```xml
<appSettings>
  <!-- ===== Configuración Azure Entra ID (Azure AD) ===== -->
  <add key="ida:TenantId" value="790c9737-0b8e-4138-a0f4-819cdc1eb64b" />
  <add key="ida:ClientId" value="08b409d6-e101-4b65-8088-2bfd770baaa7" />
  <add key="ida:ClientSecret" value="xxx" />
  <add key="ida:RedirectUri" value="http://localhost/Scato.WebMobile/signin-oidc" />
  <add key="ida:PostLogoutRedirectUri" value="http://localhost/Scato.WebMobile/" />
  <add key="ida:Authority" value="https://login.microsoftonline.com/790c9737-0b8e-4138-a0f4-819cdc1eb64b/v2.0" />
  <add key="ida:UseHttps" value="false" />
  <add key="owin:AppStartup" value="Molinos.Scato.WebMobile.Startup" />
</appSettings>
```

**Actualizar modo de autenticación:**
```xml
<system.web>
  <authentication mode="None" />
  <!-- IMPORTANTE: No usar <authorization> aquí cuando se usa OWIN -->
  <!-- OWIN maneja la autenticación a través del middleware -->
  <!-- Si necesitas denegar usuarios anónimos, usa [Authorize] en los controllers -->
</system.web>
```

**Configuración de Módulos:**
```xml
<system.webServer>
  <!-- Permitir que OWIN maneje respuestas 401 en lugar de IIS -->
  <httpErrors existingResponse="PassThrough" />
  
  <modules runAllManagedModulesForAllRequests="true">
    <remove name="FormsAuthentication" />
    
    <!-- ===== Módulos de Autenticación ADFS (Heredado - Comentado para migración a Entra ID) ===== -->
    <!--<add name="WSFederationAuthenticationModule" type="Molinos.Scato.WebMobile.Seguridad.FixedWsFederationAuthenticationModule, Molinos.Scato.WebMobile" preCondition="managedHandler" />-->
    <!--<add name="SessionAuthenticationModule" type="System.IdentityModel.Services.SessionAuthenticationModule, System.IdentityModel.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" preCondition="managedHandler" />-->
    
    <!-- ===== Módulo de Autenticación de Desarrollo ===== -->
    <!-- Módulo dummy para entorno de desarrollo y depuración. Evita el tener que utilizar Azure Entra ID -->
    <!-- Comentar esto cuando se pruebe con autenticación real de Entra ID -->
    <!--<add name="DummyAuthenticationModule" type="Molinos.Scato.WebMobile.Seguridad.DummyAuthenticationModule, Molinos.Scato.WebMobile" />-->
    
    <!-- ===== Módulo de Error Personalizado ===== -->
    <add name="AuthenticationErrorModule" type="Molinos.Scato.WebMobile.App_Start.AuthenticationErrorModule, Molinos.Scato.WebMobile" />
  </modules>
</system.webServer>
```

**Remover/Comentar secciones ADFS (Ya realizado):**
```xml
<!-- Comentar o remover: -->
<!-- <system.identityModel> ... </system.identityModel> -->
<!-- <system.identityModel.services> ... </system.identityModel.services> -->
```

**Permitir acceso anónimo a contenido estático:**
```xml
<!-- Excluir directorios de archivos estáticos de la autenticación -->
<location path="Content">
  <system.web>
    <authorization>
      <allow users="*" />
    </authorization>
  </system.web>
</location>

<location path="Scripts">
  <system.web>
    <authorization>
      <allow users="*" />
    </authorization>
  </system.web>
</location>

<location path="ErrorPages">
  <system.web>
    <authorization>
      <allow users="*" />
    </authorization>
  </system.web>
</location>
```

#### 2.3 Actualizar Módulo de Autenticación de Desarrollo
Actualizar `DummyAuthenticationModule.cs` para que coincida con los tipos de claim de Entra ID:

```csharp
using Molinos.Scato.Dominio.Seguridad;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;

namespace Molinos.Scato.WebMobile.Seguridad
{
    /// <summary>
    /// Módulo de autenticación de desarrollo que omite la autenticación de Azure Entra ID
    /// Este módulo proporciona claims falsos para propósitos de prueba sin requerir autenticación real
    /// Comentar en Web.config cuando se pruebe con autenticación real de Entra ID
    /// </summary>
    public class DummyAuthenticationModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnAuthenticateRequest;
        }

        private void OnAuthenticateRequest(object sender, System.EventArgs e)
        {
            var context = HttpContext.Current;

            // Crear claims compatibles con la estructura de Azure Entra ID
            var claims = new List<Claim>
            {
                // Claims de identidad estándar
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.NameIdentifier, "molinosagro\\lavrench"),
                
                // Claims estilo Entra ID
                new Claim("preferred_username", "lavrench@molinosagro.com"),
                new Claim(ClaimTypes.Email, "lavrench@molinosagro.com"),
                new Claim(ClaimTypes.Upn, "lavrench@molinosagro.com"),
                
                // Claims específicos de la aplicación (estos son agregados por Startup.Auth.cs en producción)
                new Claim("CentroDescripcion", "SLO"),
                new Claim("CentroId", "5"),
                new Claim("EstacionMeteorologica", "noConfigurada")
            };

            // Agregar todos los permisos para desarrollo (imita la carga de permisos desde base de datos)
            foreach (var permiso in Enum.GetValues(typeof(PermisosScato)))
            {
                claims.Add(new Claim(ClaimTypes.Role, permiso.ToString()));
            }

            var identity = new ClaimsIdentity(claims, "Development");
            context.User = new ClaimsPrincipal(identity);
        }

        public void Dispose() { }
    }
}
```

#### 2.4 Crear Controlador de Cuenta
**Archivo**: `Molinos.Scato.WebMobile\Controllers\AccountController.cs`

```csharp
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Web;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Controllers
{
    /// <summary>
    /// Controlador para manejar acciones de autenticación (iniciar sesión, cerrar sesión)
    /// </summary>
    [AllowAnonymous]
    public class AccountController : Controller
    {
        /// <summary>
        /// Inicia el inicio de sesión con Azure Entra ID
        /// </summary>
        [AllowAnonymous]
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                // Usar Url.Content para obtener URL relativa a la aplicación para la redirección
                var redirectUri = Url.Content("~/");
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = redirectUri },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else
            {
                Response.Redirect(Url.Content("~/"));
            }
        }

        /// <summary>
        /// Cierra la sesión del usuario actual tanto de la aplicación como de Azure Entra ID
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
        }

        /// <summary>
        /// Método alternativo de cierre de sesión que puede ser llamado vía GET
        /// Usar con precaución - se prefiere POST con token anti-falsificación
        /// </summary>
        [AllowAnonymous]
        public void SignOutDirect()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}
```

---

## Detalles Importantes de Implementación

### Estrategia de Extracción de Nombre de Usuario
La implementación usa una **estrategia de fallback multinivel** para extraer el nombre de usuario para búsquedas de permisos en base de datos:

1. **Primario: Claims de Nombre de Cuenta SAM On-Premises**
   - Verifica el claim `onprem_samaccountname` (más común)
   - También verifica `onprem_sam_account_name` (formato alternativo)
   - Maneja el formato `DOMAIN\username` extrayendo solo la parte del nombre de usuario

2. **Secundario: Microsoft Graph API**
   - Si el claim no está presente, consulta Graph API por `onPremisesSamAccountName`
   - Requiere permiso `User.Read` en el registro de aplicación
   - Usa el access token del flujo OIDC (`RedeemCode=true`)
   - Fallback a intercambio manual de token si es necesario (escenarios HTTPS)

3. **Terciario: Nombre de Usuario Preferido (Parseado)**
   - Extrae el nombre de usuario del claim `preferred_username` (ej., `user@domain.com` ? `user`)
   - Adecuado para usuarios solo en la nube o como último recurso

4. **Fallback Final: UPN o Name**
   - Usa `ClaimTypes.Upn` o `ClaimTypes.Name` como último recurso absoluto

### Configuración de Sesión
- **Tipo de Cookie**: Usa `SystemWebChunkingCookieManager` para manejar cookies grandes (muchos permisos)
- **Expiración**: 8 horas con expiración deslizante
- **Tipo de Autenticación**: `CookieAuthenticationDefaults.AuthenticationType`

### Consideraciones de Seguridad
- **Flujo de Código de Autorización**: Más seguro que el flujo implícito, el intercambio de tokens ocurre del lado del servidor
- **Secreto de Cliente**: Actualmente en Web.config, debería migrar a Azure Key Vault para producción
- **Aplicación de HTTPS**: Controlado vía configuración `ida:UseHttps` (afecta validación de nonce)
- **Redirección de Errores**: Manejo personalizado de errores redirige a `/ErrorPages/GenericError.html`
- **Validación de Permisos**: Usuarios sin roles se les niega acceso con mensaje de error informativo

### Configuración de Módulos HTTP
La aplicación soporta tres modos de autenticación vía Web.config:

1. **Modo Desarrollo**: `DummyAuthenticationModule` habilitado (omite Entra ID)
2. **Modo Producción**: Middleware OWIN maneja autenticación (ambos módulos comentados)
3. **Modo Heredado**: Módulos ADFS habilitados (solo para escenarios de rollback)

**Crítico**: Solo UN modo de autenticación debe estar activo a la vez.

### Registro (Logging)
- Usa **Logger inyectado por Ninject** (`ILoggerFactory` desde el resolvedor de dependencias)
- Registra eventos de autenticación, intentos de extracción de nombre de usuario, llamadas a Graph API
- Registro de errores para fallos de autenticación, permisos faltantes, etc.

---

### Fase 3: Pruebas

#### 3.1 Pruebas de Desarrollo
- [x] Probar con `DummyAuthenticationModule` habilitado
- [x] Verificar que los claims se pueblan correctamente
- [x] Probar autorización en controladores protegidos
- [x] Verificar flujo de trabajo de desarrollo sin dependencia de Entra ID

#### 3.2 Pruebas de Integración
1. Deshabilitar `DummyAuthenticationModule` en Web.config
2. Habilitar autenticación de Entra ID (asegurar que `ida:UseHttps` esté configurado apropiadamente)
3. Probar flujo de inicio de sesión con cuenta real de Entra ID
4. Verificar extracción de nombre de usuario (claim onprem_samaccountname o fallback a Graph API)
5. Verificar que los permisos se cargan desde base de datos vía `ListarPermisosPorUsuario()`
6. Verificar que se agregan claims de centro (CentroDescripcion, CentroId, EstacionMeteorologica)
7. Probar funcionalidad de cierre de sesión (métodos POST y GET)
8. Verificar comportamiento de tiempo de espera de sesión (expiración deslizante de 8 horas)
9. Probar manejo de errores (sin permisos, fallo de autenticación)

**Notas Importantes de Prueba:**
- **HTTP vs HTTPS**: La configuración `ida:UseHttps` controla el comportamiento de `RequireNonce` en el validador de protocolo
  - `true`: Funciona solo con HTTPS (configuración de producción)
  - `false`: Funciona con HTTP y HTTPS (desarrollo/pruebas)
- **Graph API**: Probar ambos escenarios:
  - Usuarios con claim `onprem_samaccountname` en el token
  - Usuarios sin el claim (fallback a Graph API)

#### 3.3 Pruebas de Aceptación de Usuario
1. Probar con usuarios de negocio reales
2. Verificar que la autorización basada en roles funciona con atributos `[Authorize]`
3. Probar escenarios multi-centro
4. Pruebas de rendimiento para carga de claims (consultas a base de datos)
5. Probar manejo de cookies fragmentadas con usuarios que tienen muchos permisos

### Fase 4: Despliegue

#### 4.1 Lista de Verificación Pre-Despliegue
- [ ] Respaldar Web.config actual
- [ ] Documentar configuración de ADFS (para rollback)
- [ ] Verificar configuración de Registro de Aplicación
- [ ] Actualizar URIs de redirección para URL de producción
- [ ] Configurar cronograma de rotación de secreto de cliente
- [ ] Actualizar archivos de transformación de despliegue (Web.Release.config)

#### 4.2 Pasos de Despliegue
1. Desplegar cambios de código al ambiente de staging
2. Actualizar Web.config con valores de Entra ID de producción
3. Verificar que la autenticación de staging funciona
4. Programar ventana de despliegue de producción
5. Desplegar a producción
6. Monitorear logs de aplicación
7. Verificar que los inicios de sesión de usuarios son exitosos

#### 4.3 Post-Despliegue
- Monitorear logs de error para fallos de autenticación
- Recolectar retroalimentación de usuarios
- Documentar cualquier problema
- Planificar desmantelamiento de ADFS (después de período estable)

---

## Plan de Rollback

### Si la Migración Falla

#### Rollback Rápido (Solo Configuración)
1. Restaurar Web.config previo desde respaldo
2. Re-habilitar módulos WS-Federation:
   ```xml
   <add name="WSFederationAuthenticationModule" type="..." />
   <add name="SessionAuthenticationModule" type="..." />
   ```
3. Comentar registro de inicio OWIN
4. Reiniciar pool de aplicaciones

#### Rollback Completo (Código + Configuración)
1. Revertir a paquete de despliegue anterior
2. Restaurar secciones de configuración ADFS
3. Remover paquetes NuGet OWIN (si es necesario)
4. Verificar conectividad ADFS

### Monitoreo Durante Migración
- Application Insights / New Relic para excepciones
- Logs de IIS para fallos de autenticación
- Logs de base de datos para consultas de permisos
- Reportes de usuarios sobre problemas de acceso

---

## Consideraciones de Seguridad

### 1. Gestión de Secreto de Cliente
- **Actual**: Almacenado en Web.config (encriptado vía DPAPI durante despliegue)
- **Recomendación**: Migrar a Azure Key Vault en el futuro
- **Rotación**: Programar rotación trimestral

### 2. Validación de Claims
- Verificar ID de inquilino en tokens
- Validar que el claim de audiencia coincide con ClientId
- Considerar implementar validación adicional de claims

### 3. Seguridad de Sesión
- Usar HTTPS en producción (requireSsl="true")
- Habilitar encriptación de cookies
- Establecer `hideFromScript="true"` para cookies
- Considerar tiempos de espera de sesión más cortos para operaciones sensibles

### 4. Autorización
- Mantener autorización basada en roles existente
- El mapeo de claims permanece impulsado por base de datos
- No se necesitan cambios a atributos `[Authorize]`

---

## Dependencias y Compatibilidad

### Requisitos de Framework
- .NET Framework 4.7.2 (objetivo actual) ?
- ASP.NET MVC 4+ ?
- IIS 7.5+ ?

### Compatibilidad de Paquetes NuGet
| Paquete | Versión Actual | Estado |
|---------|----------------|--------|
| Microsoft.Owin | 4.2.3 | ? Instalado |
| Microsoft.Owin.Security.OpenIdConnect | 4.2.3 | ? Instalado |
| Microsoft.Owin.Security.Cookies | 4.2.3 | ? Instalado |
| Microsoft.Owin.Host.SystemWeb | 4.2.3 | ? Instalado |
| System.IdentityModel.Tokens.Jwt | 8.15.0 | ? Instalado |
| Microsoft.IdentityModel.Protocols.OpenIdConnect | 8.15.0 | ? Instalado |
| Microsoft.IdentityModel.Tokens | 8.15.0 | ? Instalado |
| Microsoft.Identity.Client | 4.81.0 | ? Instalado |
| Newtonsoft.Json | 13.0.1 | ? Instalado |

### Cambios Incompatibles
- No se esperan para controladores existentes
- Los atributos de autorización permanecen sin cambios
- Estructura de claims preservada vía transformación personalizada

---

## Estimación de Cronograma

| Fase | Duración | Estado | Notas |
|------|----------|--------|-------|
| 1. Preparación | 1-2 días | ? Completo | Instalación de paquetes, actualizaciones de configuración |
| 2. Cambios de Código | 3-5 días | ? Completo | Configuración OWIN, Startup.Auth, AccountController, DummyAuthenticationModule |
| 3. Pruebas | 5-7 días | ?? En Progreso | Pruebas de desarrollo con DummyModule, pruebas de integración Entra ID pendientes |
| 4. Despliegue | 1 día | ? Pendiente | Incluye capacidad de rollback |
| **Total** | **2-3 semanas** | **60% Completo** | Código completo, pruebas en progreso |

---

## Referencias

### Documentación de Microsoft
- [Autenticación Azure AD para .NET Framework](https://docs.microsoft.com/es-es/azure/active-directory/develop/quickstart-v2-aspnet-webapp)
- [Middleware OWIN](https://docs.microsoft.com/es-es/aspnet/aspnet/overview/owin-and-katana/)
- [OpenID Connect con Azure AD](https://docs.microsoft.com/es-es/azure/active-directory/develop/v2-protocols-oidc)

### Recursos Internos
- Servidor ADFS: `bfdev271.baunet.local`
- Portal de Azure: [portal.azure.com](https://portal.azure.com)
- Registro de Aplicación: [Enlace por agregar]

---

## Contacto y Soporte

**Líderes Técnicos:**
- Autenticación: [Nombre]
- Base de Datos/Permisos: [Nombre]
- DevOps/Despliegue: [Nombre]

**Soporte Durante la Migración:**
- Crear incidentes en [sistema de tickets]
- Ruta de escalamiento: [definir escalamiento]

---

## Historial de Revisiones

| Fecha | Versión | Autor | Cambios |
|-------|---------|-------|---------|
| 2024-01-XX | 1.0 | fpagano | Plan de migración inicial creado |
| 2024-XX-XX | 2.0 | fpagano | **Actualizado con implementación real** - Agregado código completo para Startup.cs, Startup.Auth.cs, AccountController, DummyAuthenticationModule; Actualizada configuración con valores reales de Registro de Aplicación Azure; Documentada estrategia de extracción de nombre de usuario (claims + fallback Graph API); Agregada sección de detalles de implementación; Actualizados paquetes NuGet con versiones reales; Actualizado cronograma con progreso actual |
| 2024-XX-XX | 2.1 | fpagano | **Versión en español** - Traducido completamente al español para documentación interna |

---

## Apéndice A: Comparación de Configuración

### ADFS (Actual)
```xml
<add key="ida:Issuer" value="https://bfdev271.baunet.local/adfs/ls/" />
<add key="ida:FederationMetadataLocation" value="https://bfdev271/FederationMetadata/..." />
```

### Entra ID (Objetivo)
```xml
<add key="ida:TenantId" value="{tenant-id}" />
<add key="ida:ClientId" value="{client-id}" />
<add key="ida:Authority" value="https://login.microsoftonline.com/{tenant-id}" />
```

### Diferencias Clave
- **Protocolo**: WS-Federation ? OpenID Connect
- **Formato de Token**: SAML ? JWT
- **Metadatos**: XML de Federación ? JSON de configuración OpenID
- **Fuente de Claims**: ADFS ? Entra ID + Enriquecimiento desde base de datos

---

## Apéndice B: Escenarios de Prueba

### Escenario 1: Inicio de Sesión Exitoso
1. Usuario navega a la aplicación
2. Redirigido al inicio de sesión de Entra ID
3. Ingresa credenciales
4. Redirigido de vuelta a la aplicación
5. Permisos cargados desde base de datos
6. Claims de centro agregados
7. Usuario ve contenido autorizado

### Escenario 2: Usuario No Autorizado
1. Usuario se autentica con Entra ID
2. No se encuentran permisos en base de datos
3. Usuario ve mensaje de "no autorizado"
4. No puede acceder a recursos protegidos

### Escenario 3: Tiempo de Espera de Sesión
1. Usuario se autentica exitosamente
2. Sesión expira después de 8 horas
3. Usuario hace clic en enlace protegido
4. Redirigido a Entra ID (autenticación silenciosa si es posible)
5. Retornado a la solicitud original

### Escenario 4: Cierre de Sesión
1. Usuario hace clic en cerrar sesión
2. La aplicación limpia la sesión local
3. Redirigido al cierre de sesión de Entra ID
4. Entra ID limpia su sesión
5. Usuario redirigido a URI post-cierre de sesión
6. No puede acceder a recursos protegidos sin re-autenticación

---

**FIN DEL DOCUMENTO**
