# Guía de Desarrollo - Autenticación con Azure Entra ID
## Molinos.Scato.WebMobile

---

## 📋 Tabla de Contenidos

1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Conceptos Fundamentales](#conceptos-fundamentales)
3. [Arquitectura de la Solución](#arquitectura-de-la-solución)
4. [Flujo de Autenticación](#flujo-de-autenticación)
5. [Componentes del Sistema](#componentes-del-sistema)
6. [Desarrollo Local](#desarrollo-local)
7. [Guía de Código](#guía-de-código)
8. [Troubleshooting](#troubleshooting)
9. [Buenas Prácticas](#buenas-prácticas)
10. [Preguntas Frecuentes](#preguntas-frecuentes)

---

## Resumen Ejecutivo

### ¿Qué Cambió?

**ANTES (ADFS):**
- Protocolo: WS-Federation
- Tokens: SAML
- Autenticación: Servidor ADFS on-premises (`bfdev271.baunet.local`)
- Gestión: Windows Identity Foundation (WIF)

**AHORA (Entra ID):**
- Protocolo: OpenID Connect (OIDC)
- Tokens: JWT (JSON Web Tokens)
- Autenticación: Azure Entra ID (cloud)
- Gestión: OWIN Middleware

### ¿Por Qué Este Cambio?

✅ **Ventajas:**
- Soporte moderno de Microsoft (ADFS está deprecado)
- Mayor seguridad (flujo de código de autorización)
- Integración nativa con Microsoft 365
- Mejor rendimiento y escalabilidad
- Soporte para autenticación multi-factor (MFA)
- Gestión centralizada en Azure Portal

### Impacto para Desarrolladores

⚠️ **Lo que NO cambia:**
- Atributos `[Authorize]` en controllers
- Estructura de permisos en base de datos
- Lógica de negocio de la aplicación
- Claims de centro (CentroId, CentroDescripcion)

✅ **Lo que cambia:**
- Modo de configuración en `Web.config`
- Módulos HTTP para autenticación
- Flujo de login/logout
- Gestión de sesiones (ahora con cookies OWIN)

---

## Conceptos Fundamentales

### OpenID Connect (OIDC)

OpenID Connect es una capa de identidad construida sobre OAuth 2.0. Permite a las aplicaciones verificar la identidad de los usuarios y obtener información básica de perfil.

**Conceptos Clave:**

```
┌─────────────────────────────────────────────────────────────┐
│                    OAUTH 2.0 + OIDC                         │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Authorization Code Flow (lo que usamos):                   │
│                                                             │
│  1. Usuario → App: "Quiero entrar"                          │
│  2. App → Entra ID: "Redirige al usuario para login"        │
│  3. Usuario → Entra ID: Ingresa credenciales                │
│  4. Entra ID → App: Devuelve código de autorización         │
│  5. App → Entra ID: Canjea código por tokens                │
│  6. Entra ID → App: Devuelve access_token + id_token        │
│  7. App: Crea sesión local con los tokens                   │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Tipos de Tokens

#### 1. **ID Token** (JWT)
- Contiene información sobre el usuario (claims)
- Usado para autenticación
- Ejemplo de claims:
  ```json
  {
    "oid": "abc123...",
    "preferred_username": "fpagano@molinosagro.com.ar",
    "name": "Fernando Pagano",
    "onprem_samaccountname": "fpagano",
    "email": "fpagano@molinosagro.com.ar"
  }
  ```

#### 2. **Access Token**
- Usado para llamar APIs (como Microsoft Graph)
- Tiene un scope (permisos)
- En nuestra app: scope `User.Read` para Graph API

#### 3. **Refresh Token** (opcional)
- Permite renovar tokens sin pedir credenciales nuevamente
- No lo usamos actualmente (sesión de 8 horas es suficiente)

### Claims (Reclamaciones)

Son pares clave-valor que contienen información sobre el usuario:

```csharp
// Claims que vienen de Entra ID:
preferred_username: "fpagano@molinosagro.com.ar"
name: "Fernando Pagano"
onprem_samaccountname: "fpagano"  // ← Lo más importante para nosotros

// Claims que agregamos nosotros (desde BD):
ClaimTypes.Role: "PermisosScato.VerCartasPorte"
ClaimTypes.Role: "PermisosScato.EditarPesadas"
CentroId: "5"
CentroDescripcion: "SLO"
EstacionMeteorologica: "SLO_METEO"
```

---

## Arquitectura de la Solución

### Componentes Principales

```
┌─────────────────────────────────────────────────────────────────┐
│                      MOLINOS.SCATO.WEBMOBILE                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────┐         ┌──────────────────┐               │
│  │   Startup.cs    │────────▶│ Startup.Auth.cs  │               │
│  │  (OWIN Entry)   │         │  (Auth Config)   │               │
│  └─────────────────┘         └──────────────────┘               │
│                                      │                          │
│                                      │ Configura                │
│                                      ▼                          │
│                    ┌─────────────────────────────┐              │
│                    │  OWIN Middleware Pipeline   │              │
│                    ├─────────────────────────────┤              │
│                    │ 1. Cookie Authentication    │              │
│                    │ 2. OpenID Connect Auth      │              │
│                    └─────────────────────────────┘              │
│                              │                                  │
│          ┌──────────────────┼──────────────────┐                │
│          ▼                   ▼                  ▼               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐           │
│  │ OnAuthFailed │  │ OnTokenValid │  │ OnCodeRecv   │           │
│  │   Handler    │  │   Handler    │  │   Handler    │           │
│  └──────────────┘  └──────────────┘  └──────────────┘           │
│                            │                                    │
│                            │ Extrae username                    │
│                            │ Carga permisos                     │
│                            ▼                                    │
│                  ┌──────────────────┐                           │
│                  │  Claims Identity │                           │
│                  │   (con roles)    │                           │
│                  └──────────────────┘                           │
│                            │                                    │
│                            ▼                                    │
│                  ┌──────────────────┐                           │
│                  │   Controllers    │                           │
│                  │  [Authorize]     │                           │
│                  └──────────────────┘                           │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
         │                               │
         │ Login/Logout                  │ User.Read API
         ▼                               ▼
┌──────────────────┐          ┌──────────────────┐
│  Azure Entra ID  │          │ Microsoft Graph  │
│  (OIDC Provider) │          │       API        │
└──────────────────┘          └──────────────────┘
```

### Flujo de Datos

```
Usuario                 App                 Entra ID           Graph API        Base de Datos
  │                      │                      │                  │                  │
  │──(1) GET /Index ────▶│                      │                  │                  │
  │                      │                      │                  │                  │
  │                      │──(2) Challenge──────▶│                  │                  │
  │◀─────────────────────│                      │                  │                  │
  │                                              │                  │                  │
  │──(3) Login en Entra ID ─────────────────────▶│                  │                  │
  │◀─────────────────────────────────────────────│                  │                  │
  │                                              │                  │                  │
  │──(4) Redirect con código ───────────────────▶│                  │                  │
  │                      │                      │                  │                  │
  │                      │──(5) Canjea código──▶│                  │                  │
  │                      │◀─────tokens──────────│                  │                  │
  │                      │                      │                  │                  │
  │                      │──(6) GET User Profile───────────────────▶│                  │
  │                      │◀─────onPremisesSAM───────────────────────│                  │
  │                      │                                                             │
  │                      │──(7) Query permisos por username ────────────────────────────▶│
  │                      │◀─────Lista de permisos──────────────────────────────────────│
  │                      │                                                             │
  │                      │──(8) Crea Cookie con Claims                                 │
  │◀─────────────────────│                                                             │
  │                      │                                                             │
  │──(9) GET /Index ────▶│                                                             │
  │◀────Home Page────────│                                                             │
```

---

## Flujo de Autenticación

### Paso 1: Usuario Intenta Acceder

```csharp
// El usuario navega a: http://localhost/Scato.WebMobile/

// Controller tiene [Authorize]
[Authorize]
public class IndexController : Controller
{
    public ActionResult Index()
    {
        // Si no está autenticado, OWIN intercepta aquí
        return View();
    }
}
```

### Paso 2: OWIN Challenge

```csharp
// OWIN detecta que no hay cookie de autenticación
// Dispara un "Challenge" de OpenID Connect
// Redirige al usuario a:
https://login.microsoftonline.com/790c9737.../oauth2/v2.0/authorize?
  client_id=08b409d6...
  &redirect_uri=http://localhost/Scato.WebMobile/signin-oidc
  &response_type=code
  &scope=openid+profile+email+User.Read
  &state=...
  &nonce=...
```

### Paso 3: Usuario Se Autentica en Entra ID

```
┌────────────────────────────────────────┐
│   login.microsoftonline.com            │
├────────────────────────────────────────┤
│                                        │
│  [Molinos Agro]                        │
│                                        │
│  Usuario: fpagano@molinosagro.com.ar   │
│  Password: ************                │
│                                        │
│  [Iniciar Sesión]                      │
│                                        │
└────────────────────────────────────────┘
```

### Paso 4: Entra ID Redirige con Código

```
// Entra ID redirige de vuelta a nuestra app con un código:
http://localhost/Scato.WebMobile/signin-oidc?
  code=0.AX8AN5...  ← Código de autorización
  &state=...
```

### Paso 5: App Canjea Código por Tokens

```csharp
// OWIN automáticamente (porque RedeemCode = true):
// POST https://login.microsoftonline.com/.../oauth2/v2.0/token
// Body:
//   grant_type=authorization_code
//   code=0.AX8AN5...
//   client_id=08b409d6...
//   client_secret=1BD8Q~...
//   redirect_uri=http://localhost/Scato.WebMobile/signin-oidc

// Respuesta:
{
  "access_token": "eyJ0eXAi...",   // Para llamar Graph API
  "id_token": "eyJ0eXAiOi...",     // Info del usuario
  "expires_in": 3600
}
```

### Paso 6: OnSecurityTokenValidated - Nuestro Código Personalizado

```csharp
// En Startup.Auth.cs:
private async Task OnSecurityTokenValidated(...)
{
    var identity = context.AuthenticationTicket.Identity;

    // 1. Extraer username
    var username = await ExtractUsername(identity, context);
    // Resultado: "fpagano"

    // 2. Cargar permisos desde BD
    LoadUserPermissionsAndClaims(identity, username);
    
    // Ahora identity tiene:
    // - Claims de Entra ID (name, email, etc.)
    // - Claims de roles (PermisosScato.*)
    // - Claims de centro (CentroId, CentroDescripcion, etc.)

    // 3. Validar que tenga al menos un rol
    if (!hasRoles) {
        // Redirigir a página de error
    }

    // 4. Actualizar el ticket de autenticación
    context.AuthenticationTicket = new AuthenticationTicket(identity, ...);
}
```

### Paso 7: Cookie de Sesión Creada

```csharp
// OWIN crea una cookie encriptada:
// Nombre: .AspNet.Cookies
// Valor: [ENCRYPTED] (contiene todos los claims)
// HttpOnly: true
// Secure: true (en producción)
// Expira: 8 horas (sliding)

// El navegador guarda esta cookie
// En requests subsecuentes, OWIN la desencripta y reconstruye el ClaimsPrincipal
```

### Paso 8: Usuario Accede a la App

```csharp
// Ahora el usuario puede acceder a controllers protegidos
[Authorize]
public class PesadasOnlineController : Controller
{
    public ActionResult Index()
    {
        // User.Identity.Name = "fpagano"
        // User.IsInRole("PermisosScato.VerPesadas") = true
        
        return View();
    }
}
```

---

## Componentes del Sistema

### 1. Startup.cs (Punto de Entrada OWIN)

```csharp
// Archivo: Molinos.Scato.WebMobile\Startup.cs

[assembly: OwinStartup(typeof(Molinos.Scato.WebMobile.Startup))]

public partial class Startup
{
    public void Configuration(IAppBuilder app)
    {
        ConfigureAuth(app);  // ← Llama a Startup.Auth.cs
    }
}
```

**¿Qué hace?**
- Es el punto de entrada del pipeline OWIN
- Se ejecuta al iniciar la aplicación
- Configura el middleware de autenticación

### 2. Startup.Auth.cs (Configuración de Autenticación)

```csharp
// Archivo: Molinos.Scato.WebMobile\App_Start\Startup.Auth.cs

public partial class Startup
{
    public void ConfigureAuth(IAppBuilder app)
    {
        // 1. Configurar autenticación por Cookie
        app.UseCookieAuthentication(new CookieAuthenticationOptions
        {
            ExpireTimeSpan = TimeSpan.FromHours(8),
            SlidingExpiration = true,
            CookieManager = new SystemWebChunkingCookieManager()
        });

        // 2. Configurar OpenID Connect
        app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
        {
            ClientId = "08b409d6...",
            Authority = "https://login.microsoftonline.com/.../v2.0",
            ResponseType = OpenIdConnectResponseType.Code,
            RedeemCode = true,  // ← Canjear código automáticamente
            
            Notifications = new OpenIdConnectAuthenticationNotifications
            {
                AuthenticationFailed = OnAuthenticationFailed,
                SecurityTokenValidated = OnSecurityTokenValidated,
                AuthorizationCodeReceived = OnAuthorizationCodeReceived
            }
        });
    }
}
```

**Eventos Importantes:**

#### OnAuthorizationCodeReceived
```csharp
// Se ejecuta cuando Entra ID devuelve el código de autorización
private Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification context)
{
    _log?.Info("Código recibido. RedirectUri: {0}", context.RedirectUri);
    return Task.CompletedTask;
}
```

#### OnAuthenticationFailed
```csharp
// Se ejecuta si hay un error de autenticación
private Task OnAuthenticationFailed(AuthenticationFailedNotification<...> context)
{
    _log?.Error(context.Exception, "Error de autenticación");
    context.Response.Redirect("/ErrorPages/GenericError.html?message=...");
    return Task.CompletedTask;
}
```

#### OnSecurityTokenValidated (MÁS IMPORTANTE)
```csharp
// Se ejecuta cuando el token JWT es validado exitosamente
private async Task OnSecurityTokenValidated(SecurityTokenValidatedNotification<...> context)
{
    // 1. Extraer username
    var username = await ExtractUsername(identity, context);
    
    // 2. Cargar permisos desde BD
    LoadUserPermissionsAndClaims(identity, username);
    
    // 3. Validar que tenga permisos
    if (!hasRoles) {
        // Denegar acceso
    }
    
    // 4. Actualizar ticket de autenticación
    context.AuthenticationTicket = new AuthenticationTicket(identity, ...);
}
```

### 3. ExtractUsername - Lógica de Extracción de Username

```csharp
/// <summary>
/// Estrategia de fallback multinivel para obtener el username
/// </summary>
private async Task<string> ExtractUsername(ClaimsIdentity identity, ...)
{
    // ═══════════════════════════════════════════════════════════
    // NIVEL 1: Buscar en claims del token
    // ═══════════════════════════════════════════════════════════
    var samAccountClaimTypes = new[]
    {
        "onprem_samaccountname",      // ← Claim configurado en Entra ID
        "onprem_sam_account_name",
    };

    foreach (var claimType in samAccountClaimTypes)
    {
        var username = identity.FindFirst(claimType)?.Value;
        if (!string.IsNullOrEmpty(username))
        {
            // Si tiene formato DOMAIN\username, extraer solo username
            if (username.Contains("\\"))
                username = username.Substring(username.LastIndexOf('\\') + 1);
            
            _log?.Info($"Username desde claim: {username}");
            return username;  // ← Salida exitosa
        }
    }

    // ═══════════════════════════════════════════════════════════
    // NIVEL 2: Consultar Microsoft Graph API
    // ═══════════════════════════════════════════════════════════
    try
    {
        var username = await GetUsernameFromGraphApi(context);
        if (!string.IsNullOrEmpty(username))
        {
            _log?.Info($"Username desde Graph API: {username}");
            return username;  // ← Salida exitosa
        }
    }
    catch (Exception ex)
    {
        _log?.Error($"Error Graph API: {ex.Message}");
    }

    // ═══════════════════════════════════════════════════════════
    // NIVEL 3: Parsear preferred_username
    // ═══════════════════════════════════════════════════════════
    var preferredUsername = identity.FindFirst("preferred_username")?.Value;
    if (!string.IsNullOrEmpty(preferredUsername))
    {
        // Extraer "fpagano" de "fpagano@molinosagro.com.ar"
        var atIndex = preferredUsername.IndexOf('@');
        var username = atIndex > 0 
            ? preferredUsername.Substring(0, atIndex) 
            : preferredUsername;
        
        _log?.Info($"Username desde preferred_username: {username}");
        return username;  // ← Salida exitosa
    }

    // ═══════════════════════════════════════════════════════════
    // NIVEL 4: Fallback final (UPN o Name)
    // ═══════════════════════════════════════════════════════════
    var finalUsername = identity.FindFirst(ClaimTypes.Upn)?.Value ??
                       identity.FindFirst(ClaimTypes.Name)?.Value;
    
    _log?.Info($"Username desde UPN/Name: {finalUsername}");
    return finalUsername;
}
```

**Diagrama de Decisión:**

```
¿Tiene claim "onprem_samaccountname"?
     │
     ├─ SÍ ──────▶ Extraer → Limpiar DOMAIN\ → RETURN username
     │
     └─ NO ──────▶ ¿Graph API disponible?
                        │
                        ├─ SÍ ──▶ Consultar Graph → RETURN onPremisesSamAccountName
                        │
                        └─ NO ──▶ ¿Tiene preferred_username?
                                      │
                                      ├─ SÍ ──▶ Parsear (quitar @domain) → RETURN username
                                      │
                                      └─ NO ──▶ RETURN UPN o Name
```

### 4. GetUsernameFromGraphApi - Consulta a Graph API

```csharp
/// <summary>
/// Consulta Microsoft Graph API para obtener onPremisesSamAccountName
/// </summary>
private async Task<string> GetUsernameFromGraphApi(...)
{
    // 1. Obtener Object ID del usuario desde los claims
    var objectId = identity.FindFirst(
        "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
    
    if (string.IsNullOrEmpty(objectId))
        return null;

    // 2. Obtener access token para Graph API
    var accessToken = await GetGraphAccessToken(context);
    
    if (string.IsNullOrEmpty(accessToken))
        return null;

    // 3. Llamar a Graph API
    using (var httpClient = new HttpClient())
    {
        httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", accessToken);
        
        var graphUrl = 
            $"https://graph.microsoft.com/v1.0/users/{objectId}" +
            "?$select=onPremisesSamAccountName,userPrincipalName,displayName";

        var response = await httpClient.GetAsync(graphUrl);
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var user = JObject.Parse(json);
            
            return user["onPremisesSamAccountName"]?.ToString();
        }
    }
    
    return null;
}
```

**Respuesta de Graph API:**

```json
{
  "displayName": "Fernando Pagano",
  "userPrincipalName": "fpagano@molinosagro.com.ar",
  "onPremisesSamAccountName": "fpagano"  ← Lo que necesitamos
}
```

### 5. LoadUserPermissionsAndClaims - Carga desde Base de Datos

```csharp
/// <summary>
/// Carga permisos y claims adicionales desde la base de datos
/// </summary>
private void LoadUserPermissionsAndClaims(ClaimsIdentity identity, string username)
{
    var repo = DependencyResolver.Current.GetService<IServicioRepositorio>();
    var config = DependencyResolver.Current.GetService<IConfiguracionProvider>();

    // ═══════════════════════════════════════════════════════════
    // 1. CARGAR PERMISOS DESDE BASE DE DATOS
    // ═══════════════════════════════════════════════════════════
    var permisos = repo.ListarPermisosPorUsuario(username);
    // Ejemplo resultado:
    // [
    //   { Codigo: PermisosScato.VerCartasPorte },
    //   { Codigo: PermisosScato.EditarPesadas },
    //   { Codigo: PermisosScato.GenerarInformes }
    // ]

    foreach (var permiso in permisos)
    {
        if (permiso?.Codigo != null)
        {
            // Agregar como claim de ROL
            identity.AddClaim(
                new Claim(ClaimTypes.Role, permiso.Codigo.Value.ToString())
            );
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 2. CARGAR CLAIMS DE CENTRO
    // ═══════════════════════════════════════════════════════════
    var centro = repo.ObtenerCentroPorCodigoSap(
        config.AppSettings["CodigoSapSanLorenzo"]  // "1029"
    );

    if (centro != null)
    {
        identity.AddClaim(new Claim("CentroDescripcion", centro.Descripcion ?? ""));
        identity.AddClaim(new Claim("CentroId", centro.Id.ToString()));
        identity.AddClaim(new Claim("EstacionMeteorologica", 
            centro.CodigoEstacionMeteorologica ?? "noConfigurada"));
    }

    _log?.Info($"Claims cargados. Total: {identity.Claims.Count()}");
}
```

**Claims Finales en Identity:**

```
Claims de Entra ID:
├─ name: "Fernando Pagano"
├─ preferred_username: "fpagano@molinosagro.com.ar"
├─ onprem_samaccountname: "fpagano"
└─ email: "fpagano@molinosagro.com.ar"

Claims de Base de Datos:
├─ ClaimTypes.Role: "PermisosScato.VerCartasPorte"
├─ ClaimTypes.Role: "PermisosScato.EditarPesadas"
├─ ClaimTypes.Role: "PermisosScato.GenerarInformes"
├─ CentroDescripcion: "SLO"
├─ CentroId: "5"
└─ EstacionMeteorologica: "SLO_METEO"
```

### 6. AccountController - Login/Logout

```csharp
// Archivo: Molinos.Scato.WebMobile\Controllers\AccountController.cs

[AllowAnonymous]
public class AccountController : Controller
{
    /// <summary>
    /// Inicia el proceso de login con Entra ID
    /// </summary>
    [AllowAnonymous]
    public void SignIn()
    {
        if (!Request.IsAuthenticated)
        {
            // Desencadena el Challenge de OIDC
            var redirectUri = Url.Content("~/");
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = redirectUri },
                OpenIdConnectAuthenticationDefaults.AuthenticationType
            );
        }
        else
        {
            // Ya está autenticado, redirigir a home
            Response.Redirect(Url.Content("~/"));
        }
    }

    /// <summary>
    /// Cierra sesión local y en Entra ID
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public void SignOut()
    {
        // Cierra sesión en OWIN y en Entra ID
        HttpContext.GetOwinContext().Authentication.SignOut(
            OpenIdConnectAuthenticationDefaults.AuthenticationType,
            CookieAuthenticationDefaults.AuthenticationType
        );
        
        // Esto hace:
        // 1. Borra la cookie local
        // 2. Redirige a https://login.microsoftonline.com/.../oauth2/logout
        // 3. Entra ID borra su sesión
        // 4. Redirige de vuelta a PostLogoutRedirectUri
    }
}
```

---

## Desarrollo Local

### Configuración de Web.config

```xml
<appSettings>
  <!-- ═══════════════════════════════════════════════════════════ -->
  <!-- CONFIGURACIÓN AZURE ENTRA ID                                -->
  <!-- ═══════════════════════════════════════════════════════════ -->
  
  <!-- ID del tenant (inquilino) de Molinos Agro -->
  <add key="ida:TenantId" value="790c9737-0b8e-4138-a0f4-819cdc1eb64b" />
  
  <!-- ID de la aplicación registrada en Azure -->
  <add key="ida:ClientId" value="08b409d6-e101-4b65-8088-2bfd770baaa7" />
  
  <!-- Secreto de la aplicación (MANTENER SEGURO) -->
  <add key="ida:ClientSecret" value="1BD..." />
  
  <!-- URL a donde Entra ID redirige después del login -->
  <add key="ida:RedirectUri" value="http://localhost/Scato.WebMobile/signin-oidc" />
  
  <!-- URL a donde Entra ID redirige después del logout -->
  <add key="ida:PostLogoutRedirectUri" value="http://localhost/Scato.WebMobile/" />
  
  <!-- Endpoint de autenticación de Entra ID -->
  <add key="ida:Authority" value="https://login.microsoftonline.com/790c9737-0b8e-4138-a0f4-819cdc1eb64b/v2.0" />
  
  <!-- Requiere HTTPS (false para desarrollo local) -->
  <add key="ida:UseHttps" value="false" />
  
  <!-- Clase de inicio OWIN -->
  <add key="owin:AppStartup" value="Molinos.Scato.WebMobile.Startup" />
</appSettings>
```

### Módulos HTTP en Web.config

```xml
<system.webServer>
  <modules runAllManagedModulesForAllRequests="true">
    <remove name="FormsAuthentication" />
    
    <!-- ═══════════════════════════════════════════════════════════ -->
    <!-- MÓDULOS ADFS (HEREDADO - COMENTADOS)                        -->
    <!-- ═══════════════════════════════════════════════════════════ -->
    <!--<add name="WSFederationAuthenticationModule" ... />-->
    <!--<add name="SessionAuthenticationModule" ... />-->
    
    <!-- ═══════════════════════════════════════════════════════════ -->
    <!-- MÓDULO DE DESARROLLO (OPCIONAL)                             -->
    <!-- Permite desarrollo sin conectarse a Entra ID                -->
    <!-- ═══════════════════════════════════════════════════════════ -->
    <add name="DummyAuthenticationModule" 
         type="Molinos.Scato.WebMobile.Seguridad.DummyAuthenticationModule, Molinos.Scato.WebMobile" />
  </modules>
</system.webServer>
```

### Opción 1: Desarrollo con DummyAuthenticationModule

**¿Cuándo usar?**
- Desarrollo local sin conexión a internet
- Pruebas rápidas sin autenticación real
- No necesitas probar el flujo de login

**Configuración:**

```xml
<!-- EN WEB.CONFIG: Habilitar DummyAuthenticationModule -->
<add name="DummyAuthenticationModule" 
     type="Molinos.Scato.WebMobile.Seguridad.DummyAuthenticationModule, Molinos.Scato.WebMobile" />
```

**¿Qué hace?**

```csharp
// DummyAuthenticationModule.cs
public class DummyAuthenticationModule : IHttpModule
{
    private void OnAuthenticateRequest(object sender, EventArgs e)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.NameIdentifier, "molinosagro\\lavrench"),
            new Claim("preferred_username", "lavrench@molinosagro.com"),
            new Claim("CentroDescripcion", "SLO"),
            new Claim("CentroId", "5"),
        };

        // ¡AGREGAR TODOS LOS PERMISOS!
        foreach (var permiso in Enum.GetValues(typeof(PermisosScato)))
        {
            claims.Add(new Claim(ClaimTypes.Role, permiso.ToString()));
        }

        var identity = new ClaimsIdentity(claims, "Development");
        HttpContext.Current.User = new ClaimsPrincipal(identity);
    }
}
```

**Ventajas:**
- ✅ No requiere conexión a Entra ID
- ✅ Inicio rápido
- ✅ Todos los permisos habilitados

**Desventajas:**
- ❌ No prueba el flujo real de autenticación
- ❌ No valida integración con Entra ID

### Opción 2: Desarrollo con Entra ID Real

**¿Cuándo usar?**
- Probar flujo completo de autenticación
- Validar integración con Graph API
- Debugging de problemas de claims

**Configuración:**

```xml
<!-- EN WEB.CONFIG: Comentar DummyAuthenticationModule -->
<!--<add name="DummyAuthenticationModule" ... />-->
```

**Requisitos:**
1. Cuenta de usuario en Entra ID de Molinos Agro
2. Usuario configurado en la base de datos con permisos
3. Conexión a internet

**Flujo:**
1. Navegar a `http://localhost/Scato.WebMobile/`
2. Serás redirigido a login de Entra ID
3. Ingresar credenciales corporativas
4. Entra ID te redirige de vuelta
5. La app carga tus permisos desde BD

---

## Guía de Código

### Cómo Acceder a la Información del Usuario

#### En Controllers

```csharp
public class MiController : Controller
{
    public ActionResult Index()
    {
        // ═══════════════════════════════════════════════════════════
        // INFORMACIÓN BÁSICA DEL USUARIO
        // ═══════════════════════════════════════════════════════════
        
        // Nombre del usuario
        string username = User.Identity.Name;  // "fpagano"
        
        // ¿Está autenticado?
        bool isAuth = User.Identity.IsAuthenticated;  // true
        
        // ═══════════════════════════════════════════════════════════
        // VERIFICAR PERMISOS
        // ═══════════════════════════════════════════════════════════
        
        // Verificar un permiso específico
        bool puedeVerCartasPorte = User.IsInRole("PermisosScato.VerCartasPorte");
        
        // Verificar múltiples permisos
        bool tieneAccesoEspecial = 
            User.IsInRole("PermisosScato.EditarPesadas") &&
            User.IsInRole("PermisosScato.GenerarInformes");
        
        // ═══════════════════════════════════════════════════════════
        // ACCEDER A CLAIMS PERSONALIZADOS
        // ═══════════════════════════════════════════════════════════
        
        var claimsPrincipal = User as ClaimsPrincipal;
        
        // Obtener Centro ID
        string centroId = claimsPrincipal
            .FindFirst("CentroId")?.Value;  // "5"
        
        // Obtener Centro Descripción
        string centro = claimsPrincipal
            .FindFirst("CentroDescripcion")?.Value;  // "SLO"
        
        // Obtener Email
        string email = claimsPrincipal
            .FindFirst(ClaimTypes.Email)?.Value;  // "fpagano@molinosagro.com.ar"
        
        // ═══════════════════════════════════════════════════════════
        // LISTAR TODOS LOS CLAIMS
        // ═══════════════════════════════════════════════════════════
        
        var allClaims = claimsPrincipal.Claims.Select(c => new {
            Type = c.Type,
            Value = c.Value
        }).ToList();
        
        return View();
    }
}
```

#### En Views (Razor)

```razor
@* ═══════════════════════════════════════════════════════════ *@
@* MOSTRAR NOMBRE DEL USUARIO                                  *@
@* ═══════════════════════════════════════════════════════════ *@

<div>Bienvenido, @User.Identity.Name</div>

@* ═══════════════════════════════════════════════════════════ *@
@* MOSTRAR/OCULTAR SEGÚN PERMISO                               *@
@* ═══════════════════════════════════════════════════════════ *@

@if (User.IsInRole("PermisosScato.VerCartasPorte"))
{
    <a href="@Url.Action("Index", "CartasPorte")">Ver Cartas de Porte</a>
}

@if (User.IsInRole("PermisosScato.EditarPesadas"))
{
    <button>Editar Pesadas</button>
}

@* ═══════════════════════════════════════════════════════════ *@
@* ACCEDER A CLAIMS PERSONALIZADOS                             *@
@* ═══════════════════════════════════════════════════════════ *@

@{
    var claimsPrincipal = User as System.Security.Claims.ClaimsPrincipal;
    var centro = claimsPrincipal.FindFirst("CentroDescripcion")?.Value;
}

<div>Centro: @centro</div>
```

### Proteger Controllers y Actions

#### Proteger todo un Controller

```csharp
// Todos los métodos requieren autenticación
[Authorize]
public class CartasPorteController : Controller
{
    public ActionResult Index()
    {
        // Solo usuarios autenticados pueden acceder
        return View();
    }
}
```

#### Proteger con Permisos Específicos

```csharp
// Requiere un permiso específico
[Authorize(Roles = "PermisosScato.VerCartasPorte")]
public class CartasPorteController : Controller
{
    public ActionResult Index()
    {
        // Solo usuarios con el permiso pueden acceder
        return View();
    }
}
```

#### Múltiples Permisos (OR)

```csharp
// El usuario necesita AL MENOS UNO de estos permisos
[Authorize(Roles = "PermisosScato.VerCartasPorte,PermisosScato.EditarCartasPorte")]
public ActionResult Ver(int id)
{
    return View();
}
```

#### Múltiples Permisos (AND)

```csharp
// El usuario necesita AMBOS permisos
[Authorize(Roles = "PermisosScato.VerCartasPorte")]
[Authorize(Roles = "PermisosScato.EditarCartasPorte")]
public ActionResult Editar(int id)
{
    return View();
}
```

#### Permitir Acceso Anónimo en Controller Protegido

```csharp
[Authorize]
public class CartasPorteController : Controller
{
    // Este método requiere autenticación (hereda del controller)
    public ActionResult Index()
    {
        return View();
    }

    // Este método permite acceso anónimo
    [AllowAnonymous]
    public ActionResult Publico()
    {
        return View();
    }
}
```

### Verificar Permisos en Código

```csharp
public ActionResult Eliminar(int id)
{
    // Verificar permiso manualmente
    if (!User.IsInRole("PermisosScato.EliminarCartasPorte"))
    {
        return new HttpStatusCodeResult(HttpStatusCode.Forbidden, 
            "No tiene permisos para eliminar cartas de porte");
    }

    // Lógica de eliminación...
    
    return RedirectToAction("Index");
}
```

### Logging de Autenticación

```csharp
// En Startup.Auth.cs ya tenemos logging configurado:

// Obtener el logger
var loggerFactory = DependencyResolver.Current.GetService<ILoggerFactory>();
_log = loggerFactory?.GetCurrentClassLogger();

// Usar el logger
_log?.Info("Usuario autenticado: {0}", username);
_log?.Error(ex, "Error al consultar Graph API");
_log?.Warn("Usuario sin permisos: {0}", username);
```

**Ver logs:**
- Desarrollo: Archivos de log en carpeta `Logs/`
- Producción: Application Insights / NewRelic

---

## Troubleshooting

### Problema 1: "The nonce cookie was not found"

**Síntoma:**
```
Error de autenticación: The nonce cookie was not found
```

**Causa:**
- Configuración `RequireNonce = true` pero la app está en HTTP (no HTTPS)

**Solución:**

```xml
<!-- En Web.config, asegurarse: -->
<add key="ida:UseHttps" value="false" />
```

```csharp
// En Startup.Auth.cs, verificar:
optionsOpenIdAuth.ProtocolValidator.RequireNonce = UseHttpsConfig;  // false para HTTP
```

### Problema 2: "No se encontró el ObjectId en los claims"

**Síntoma:**
```
Error al intentar obtener username desde Graph API: No se encontró el ObjectId
```

**Causa:**
- El token de ID no incluye el claim `objectidentifier`

**Solución:**
1. Verificar en Azure Portal que el token incluye el claim
2. Agregar opcional claim en Token Configuration
3. Como fallback, usar `preferred_username`

### Problema 3: Usuario sin permisos

**Síntoma:**
```
Error: El usuario no tiene permisos asignados para esta aplicación
```

**Causa:**
- El usuario no existe en la tabla de permisos de la BD
- La consulta `ListarPermisosPorUsuario(username)` devuelve lista vacía

**Solución:**

```sql
-- Verificar en base de datos:
SELECT * FROM Usuarios WHERE NombreUsuario = 'fpagano'

-- Si no existe, agregar:
INSERT INTO Usuarios (NombreUsuario, ...) VALUES ('fpagano', ...)

-- Agregar permisos:
INSERT INTO UsuariosPermisos (UsuarioId, PermisoId) 
VALUES (
    (SELECT Id FROM Usuarios WHERE NombreUsuario = 'fpagano'),
    (SELECT Id FROM Permisos WHERE Codigo = 12345)  -- PermisosScato.VerCartasPorte
)
```

### Problema 4: Cookie muy grande

**Síntoma:**
```
HTTP Error 400.0 - Bad Request
```

**Causa:**
- Usuario tiene demasiados permisos
- Cookie de claims excede el límite de IIS (4KB por header)

**Solución:**

Ya implementado: `SystemWebChunkingCookieManager`

```csharp
// En Startup.Auth.cs:
app.UseCookieAuthentication(new CookieAuthenticationOptions
{
    CookieManager = new SystemWebChunkingCookieManager()  // ← Fragmenta cookies grandes
});
```

### Problema 5: Redirect Loop Infinito

**Síntoma:**
- La página se queda redirigiendo constantemente
- Browser dice "Too many redirects"

**Causa:**
- Conflict entre módulos de autenticación
- `DummyAuthenticationModule` y OWIN activos simultáneamente

**Solución:**

```xml
<!-- Asegurarse de tener SOLO UNO habilitado: -->

<!-- OPCIÓN 1: Desarrollo con Dummy -->
<add name="DummyAuthenticationModule" ... />

<!-- OPCIÓN 2: Entra ID real -->
<!-- Comentar DummyAuthenticationModule -->
```

### Problema 6: "Correlation failed"

**Síntoma:**
```
Correlation failed. Unknown location
```

**Causa:**
- Cookie de correlación se perdió
- Usuario tiene cookies deshabilitadas
- Problema de dominio/path en cookies

**Solución:**

```xml
<!-- Verificar en Web.config: -->
<add key="ida:RedirectUri" value="http://localhost/Scato.WebMobile/signin-oidc" />

<!-- NO usar: -->
<!-- value="http://localhost:80/..." -->  ← Puerto explícito puede causar problemas
<!-- value="http://localhost/" -->         ← Falta path de app
```

---

## Buenas Prácticas

### 1. Seguridad del Client Secret

❌ **NO HACER:**

```csharp
// Hardcodear secreto en código
var secret = "1BD...";
```

✅ **HACER:**

```xml
<!-- Mantener en Web.config -->
<add key="ida:ClientSecret" value="1BD8Q~_..." />
```

```xml
<!-- En producción, usar Web.Release.config para transformar: -->
<add key="ida:ClientSecret" 
     value="#{ClientSecret}#"  
     xdt:Transform="SetAttributes" 
     xdt:Locator="Match(key)" />
```

### 2. Manejo de Errores de Autenticación

✅ **HACER:**

```csharp
private Task OnAuthenticationFailed(...)
{
    // 1. Loguear el error
    _log?.Error(context.Exception, "Error de autenticación Entra ID");
    
    // 2. Redirigir a página de error amigable
    context.HandleResponse();
    var errorUrl = $"{pathBase}/ErrorPages/GenericError.html?message=" +
        Uri.EscapeDataString(context.Exception.Message);
    context.Response.Redirect(errorUrl);
    
    return Task.CompletedTask;
}
```

### 3. Validación de Claims

✅ **HACER:**

```csharp
private async Task OnSecurityTokenValidated(...)
{
    var username = await ExtractUsername(identity, context);
    
    // Validar que obtuvimos username
    if (string.IsNullOrEmpty(username))
    {
        _log?.Warn("No se pudo determinar el usuario");
        // Manejar error...
        return;
    }
    
    LoadUserPermissionsAndClaims(identity, username);
    
    // Validar que el usuario tiene al menos un permiso
    var hasRoles = identity.Claims.Any(c => c.Type == ClaimTypes.Role);
    if (!hasRoles)
    {
        _log?.Warn("Usuario {0} sin permisos", username);
        // Denegar acceso...
        return;
    }
}
```

### 4. Logging Apropiado

✅ **HACER:**

```csharp
// Niveles de log apropiados:

// INFO: Flujo normal
_log?.Info("Usuario autenticado: {0}", username);
_log?.Info("Claims cargados. Total: {0}", identity.Claims.Count());

// WARN: Situaciones anómalas pero no errores
_log?.Warn("Usuario {0} sin permisos en BD", username);
_log?.Warn("Claim onprem_samaccountname no encontrado, usando fallback");

// ERROR: Errores reales
_log?.Error(ex, "Error al consultar Graph API");
_log?.Error("No se pudo obtener access token para Graph API");
```

### 5. Configuración por Ambiente

✅ **HACER:**

```xml
<!-- Web.config (Desarrollo) -->
<add key="ida:RedirectUri" value="http://localhost/Scato.WebMobile/signin-oidc" />
<add key="ida:UseHttps" value="false" />

<!-- Web.QA.config (Transformación) -->
<add key="ida:RedirectUri" 
     value="http://vicscatoqa/Scato.WebMobile/signin-oidc" 
     xdt:Transform="SetAttributes" 
     xdt:Locator="Match(key)" />
<add key="ida:UseHttps" 
     value="true" 
     xdt:Transform="SetAttributes" 
     xdt:Locator="Match(key)" />

<!-- Web.Release.config (Producción) -->
<add key="ida:RedirectUri" 
     value="https://scato.molinosagro.com.ar/signin-oidc" 
     xdt:Transform="SetAttributes" 
     xdt:Locator="Match(key)" />
<add key="ida:UseHttps" 
     value="true" 
     xdt:Transform="SetAttributes" 
     xdt:Locator="Match(key)" />
```

### 6. Testing de Permisos

✅ **HACER:**

```csharp
[TestClass]
public class AuthorizationTests
{
    [TestMethod]
    public void Usuario_Con_Permiso_Puede_Acceder()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "fpagano"),
            new Claim(ClaimTypes.Role, "PermisosScato.VerCartasPorte")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        
        // Act
        bool tienePermiso = principal.IsInRole("PermisosScato.VerCartasPorte");
        
        // Assert
        Assert.IsTrue(tienePermiso);
    }
}
```

---

## Preguntas Frecuentes

### ¿Qué pasa si Entra ID está caído?

**Respuesta:**
- Los usuarios ya autenticados pueden seguir trabajando (tienen cookie válida por 8 horas)
- Nuevos logins fallarán
- No hay fallback automático a ADFS
- Considerar implementar página de "Servicio Temporalmente No Disponible"

### ¿Cómo rotar el Client Secret?

**Pasos:**

1. **En Azure Portal:**
   - Ir a App Registration → Certificates & secrets
   - Crear nuevo client secret
   - Copiar el valor (solo visible al crearlo)

2. **Actualizar Web.config:**
   ```xml
   <add key="ida:ClientSecret" value="NUEVO_SECRETO" />
   ```

3. **Desplegar:**
   - El secreto viejo sigue funcionando hasta que expire
   - Hacer deployment progresivo
   - Eliminar secreto viejo después de confirmar que el nuevo funciona

**Frecuencia Recomendada:** Cada 3-6 meses

### ¿Puedo tener usuarios solo en la nube (sin AD on-prem)?

**Respuesta:** Sí, pero:

- No tendrán claim `onprem_samaccountname`
- La app usará fallback a `preferred_username` o `UPN`
- Funcionará si el username parseado coincide con la BD
- Ejemplo: `fpagano@molinosagro.com.ar` → extrae `fpagano`

### ¿Cómo debugging de claims?

**Método 1: En código**

```csharp
public ActionResult DebugClaims()
{
    var principal = User as ClaimsPrincipal;
    var claims = principal.Claims.Select(c => new {
        Type = c.Type,
        Value = c.Value
    }).ToList();
    
    return Json(claims, JsonRequestBehavior.AllowGet);
}
```

**Método 2: En logs**

```csharp
// En OnSecurityTokenValidated:
_log?.Info("Claims del usuario:");
foreach (var claim in identity.Claims)
{
    _log?.Info("  {0} = {1}", claim.Type, claim.Value);
}
```

**Método 3: JWT Debugger**

1. Capturar el ID Token (usando herramientas de dev del browser)
2. Ir a https://jwt.ms/
3. Pegar el token
4. Ver todos los claims decodificados

### ¿La sesión expira exactamente a las 8 horas?

**Respuesta:** No, es **sliding expiration**:

```
Usuario hace login a las 10:00
├─ Cookie expira: 18:00 (10:00 + 8 horas)
│
Usuario hace request a las 11:00
├─ Cookie se renueva: nueva expiración 19:00 (11:00 + 8 horas)
│
Usuario hace request a las 14:00
├─ Cookie se renueva: nueva expiración 22:00 (14:00 + 8 horas)
│
Si el usuario NO hace requests por 8 horas consecutivas
└─ Cookie expira, debe hacer login nuevamente
```

### ¿Puedo cambiar la duración de la sesión?

**Respuesta:** Sí:

```csharp
// En Startup.Auth.cs:
app.UseCookieAuthentication(new CookieAuthenticationOptions
{
    ExpireTimeSpan = TimeSpan.FromHours(12),  // ← Cambiar aquí
    SlidingExpiration = true
});
```

**Consideraciones:**
- Sesiones más largas: Menos logins, pero menor seguridad
- Sesiones más cortas: Mayor seguridad, pero más interrupciones
- Recomendado: 4-8 horas para apps corporativas

### ¿Qué pasa con usuarios que tienen muchos permisos?

**Respuesta:**
- Las cookies se fragmentan automáticamente (`SystemWebChunkingCookieManager`)
- No hay límite práctico de permisos
- Performance: Cargar 100+ permisos toma ~500ms en primera autenticación
- La cookie se crea una vez y se reutiliza

---

## Recursos Adicionales

### Documentación Oficial

- [Microsoft Identity Platform](https://learn.microsoft.com/en-us/entra/identity-platform/)
- [OpenID Connect en .NET Framework](https://learn.microsoft.com/en-us/aspnet/aspnet/overview/owin-and-katana/owin-oauth-20-authorization-server)
- [Microsoft Graph API](https://learn.microsoft.com/en-us/graph/overview)

### Herramientas Útiles

- **JWT.ms**: https://jwt.ms/ - Decodificar tokens JWT
- **Fiddler**: Capturar tráfico HTTP/HTTPS entre app y Entra ID
- **Azure Portal**: https://portal.azure.com - Gestión de App Registration
- **Graph Explorer**: https://developer.microsoft.com/en-us/graph/graph-explorer - Probar Graph API

### Código de Ejemplo

```csharp
// Repo oficial de Microsoft con ejemplos:
// https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2
```

### Contacto Interno

**Equipo de Desarrollo:**
- Lead: Fernando Pagano (fpagano@molinosagro.com.ar)
- DevOps: [Nombre]
- DBA: [Nombre]

**Soporte:**
- Canal de Slack: #scato-logistica
- JIRA: https://molinosagro.atlassian.net/

---

## Changelog

| Fecha | Versión | Cambios |
|-------|---------|---------|
| 2024-XX-XX | 1.0 | Creación inicial de la guía de desarrollo |

---

**FIN DEL DOCUMENTO**

✅ **Este documento es una guía viva** - Si encuentras algo que falta o está desactualizado, por favor actualízalo.

💡 **¿Tienes dudas?** - Pregunta en el canal de Slack o crea un ticket en JIRA.
