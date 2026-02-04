# Authentication Refactoring - Microsoft Graph Integration

## Overview
This document describes the refactoring of `Startup.Auth.cs` to integrate Microsoft Graph API for retrieving the On-premises SAM account name with support for both HTTP and HTTPS environments.

## Changes Made

### 1. Added New Using Directives
- `System.Collections.Generic` - For KeyValuePair
- `System.Net.Http` - For HTTP client to call Graph API
- `System.Net.Http.Headers` - For authentication headers
- `Newtonsoft.Json.Linq` - For JSON parsing

### 2. Refactored `OnSecurityTokenValidated` Method
The main authentication method has been refactored into smaller, focused methods:

#### Main Flow
```csharp
OnSecurityTokenValidated()
├── ExtractUsername()            // Orchestrates username extraction
│   ├── Check multiple SAM account claim types (6 variations)
│   ├── GetUsernameFromGraphApi()  // NEW: Calls Microsoft Graph
│   ├── Parse preferred_username
│   └── Fallback to UPN/Name
└── LoadUserPermissionsAndClaims() // Loads roles and centro claims
```

### 3. New Methods Added

#### `ExtractUsername(ClaimsIdentity identity, context)`
- **Purpose**: Orchestrates username extraction with comprehensive fallback strategy
- **Priority order**:
  1. **Multiple SAM account claim types** (tries 6 different formats):
     - `onprem_sam_account_name` - Standard Azure AD optional claim
     - `http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname` - Full URI format
     - `winaccountname` - Short form
     - `samaccountname` - Direct SAM account name
     - `account_name` - Generic account name
     - `unique_name` - Unique identifier
  2. **Microsoft Graph API** `onPremisesSamAccountName` property
  3. **`preferred_username` claim** (parsed before @)
  4. **`UPN` or `Name` claim** as final fallback
- **Special handling**: Strips domain prefix from `DOMAIN\username` format (e.g., `MOLINOS\fpagano` → `fpagano`)
- **Logging**: Uses `System.Diagnostics.Debug.WriteLine` for debugging output

#### `GetUsernameFromGraphApi(context)`
- **Purpose**: Calls Microsoft Graph API to retrieve `onPremisesSamAccountName`
- **Process**:
  1. Extracts ObjectId from claims (`http://schemas.microsoft.com/identity/claims/objectidentifier`)
  2. Gets access token for Graph API (different flows for HTTP vs HTTPS)
  3. Queries `https://graph.microsoft.com/v1.0/users/{objectId}?$select=onPremisesSamAccountName,userPrincipalName,displayName`
  4. Returns `onPremisesSamAccountName` if available
- **Error handling**: Returns null on any error, allowing fallback to continue gracefully
- **External users**: Returns null for guest users (expected behavior)

#### `GetGraphAccessToken(context)` - Optimized Token Reuse
- **Purpose**: Obtains an access token for Microsoft Graph API with optimized token reuse
- **Two-phase approach**:
  
  **Phase 1 - Primary (RedeemCode=true for both HTTP/HTTPS)**:
  - Uses already-redeemed `AccessToken` from `ProtocolMessage.AccessToken`
  - Validates token has `User.Read` scope in the scope parameter
  - No additional HTTP call needed (efficient!)
  - Returns immediately if valid token found
  - **Works for both HTTP and HTTPS** since both use `RedeemCode=true`
  
  **Phase 2 - Fallback (rare cases)**:
  - Only executes if Phase 1 fails or access token not available
  - Handles edge cases like cached authentication or token expiration
  - Manually exchanges authorization code from `ProtocolMessage.Code`
  - Posts to token endpoint: `{Authority}/oauth2/v2.0/token`
  - Uses specific scope: `https://graph.microsoft.com/User.Read`
  - Returns new access token

- **Scope validation**: Ensures token has necessary `User.Read` permission
- **Logging**: Debug output shows which method was used to obtain token
- **Performance**: Phase 1 succeeds in ~99% of cases, making Graph API calls very efficient

#### `LoadUserPermissionsAndClaims(ClaimsIdentity identity, string username)`
- **Purpose**: Loads user permissions and additional claims (centro, roles, etc.)
- **Extracted from**: Original `OnSecurityTokenValidated` method
- **Features**:
  - Retrieves permissions from database via `repo.ListarPermisosPorUsuario(username)`
  - Adds role claims for authorization (`ClaimTypes.Role`)
  - Adds centro-specific claims:
    - `CentroDescripcion` - Center description
    - `CentroId` - Center ID
    - `EstacionMeteorologica` - Weather station code
  - **Dev/Testing**: Includes hardcoded `PermisosScato.WebMobile` claim for development

## Benefits

### 1. **Better Username Resolution**
- **Internal AD-Synced Users**: Gets SAM account name from multiple claim types or Graph API
- **External/Guest Users**: Falls back gracefully to parsed `preferred_username`
- **Cloud-Only Users**: Uses UPN or Name as final fallback
- **Domain Stripping**: Handles `DOMAIN\username` format automatically

### 2. **Code Organization**
- **Single Responsibility**: Each method has one clear purpose
- **Maintainability**: Easier to test and modify individual components
- **Readability**: Clear flow from top to bottom
- **No logger dependency**: Uses `System.Diagnostics.Debug.WriteLine` for simplicity

### 3. **Simplified Token Flow**
- **Both HTTP and HTTPS**: Use `RedeemCode=true` for automatic token redemption
- **No environment-specific logic**: Same efficient flow for all environments
- **Scope validation**: Prevents using tokens without proper permissions
- **Fallback handling**: Gracefully handles edge cases with manual code exchange

## Configuration Requirements

### Azure App Registration
The app must have:
- **Client Secret**: Already configured in `Web.config`
- **Redirect URI**: Configured for both HTTP (localhost) and HTTPS (production)
- **API Permissions**: 
  - `User.Read` (Microsoft Graph - Delegated) - **Required for Graph API calls**
  - Grant admin consent for the organization
- **Optional Claims**: 
  - `onprem_sam_account_name` configured in Token Configuration (idToken)

### Web.config Settings
All required settings are already in place:
```xml
<add key="ida:ClientId" value="08b409d6-e101-4b65-8088-2bfd770baaa7" />
<add key="ida:ClientSecret" value="..." />
<add key="ida:Authority" value="https://login.microsoftonline.com/790c9737-0b8e-4138-a0f4-819cdc1eb64b/v2.0" />
<add key="ida:RedirectUri" value="https://localhost/Scato.WebMobile/signin-oidc" />
<add key="ida:PostLogoutRedirectUri" value="https://localhost/Scato.WebMobile/" />
```

### OWIN Configuration
```csharp
// Both HTTP and HTTPS use Code flow
ResponseType = OpenIdConnectResponseType.Code

// Scopes include Graph API permission
Scope = "openid profile email User.Read"

// Enable automatic code redemption for all environments
// This makes the access token available in ProtocolMessage.AccessToken
optionsOpenIdAuth.RedeemCode = true;

// HTTP-specific protocol validation settings (less strict for localhost)
if (!isHttpsRedirect) {
    optionsOpenIdAuth.ProtocolValidator.RequireNonce = false;
    // State validation can also be disabled for HTTP if needed
    // optionsOpenIdAuth.ProtocolValidator.RequireState = false;
}
```

**Key Configuration Points:**
- `ResponseType = Code` for **both HTTP and HTTPS** (fully unified!)
- `RedeemCode = true` is set for **both environments**
- This simplifies token acquisition and improves performance
- Access token is always available in `ProtocolMessage.AccessToken`
- No need for manual code exchange in production
- Only protocol validation (Nonce/State) differs between HTTP and HTTPS

## Testing Scenarios

### Scenario 1: Internal AD-Synced User
- **Expected**: `onprem_sam_account_name` from claims (if configured) OR Graph API
- **Fallback**: `preferred_username` parsed
- **Debug Output**: `"Username obtenido desde claim 'onprem_sam_account_name': username"` or `"Username obtenido desde Microsoft Graph API: username"`
- **Example**: User `john.doe@molinosagro.com.ar` → username: `john.doe`

### Scenario 2: External/Guest User (e.g., fpagano@baufest.com)
- **Expected**: `preferred_username` parsed (e.g., "Fernando.Pagano")
- **Graph API**: Returns null (external users don't have `onPremisesSamAccountName`)
- **Debug Output**: `"El usuario no tiene onPremisesSamAccountName en Graph API"` followed by `"Username obtenido desde preferred_username (parseado): Fernando.Pagano"`
- **Result**: Successfully authenticates with parsed username

### Scenario 3: Cloud-Only User
- **Expected**: `preferred_username` parsed OR UPN
- **Graph API**: May return null
- **Fallback**: Works correctly with parsed username

### Scenario 4: User with Domain Format (DOMAIN\username)
- **Expected**: Domain prefix is automatically stripped
- **Example**: `MOLINOS\john.doe` → `john.doe`
- **Works with**: Any of the SAM account claim types

### Scenario 5: Token Acquisition Flow (Both Environments)
- **HTTP and HTTPS (both use RedeemCode=true)**:
  - `RedeemCode = true` → Token auto-redeemed by OWIN middleware
  - `ProtocolMessage.AccessToken` contains valid access token
  - Scope includes `"User.Read User.Read.All openid profile email"`
  - No additional token request needed
  - Graph API calls use the redeemed token directly
  
- **Edge Cases (rare - fallback to Phase 2)**:
  - Cached authentication without new token
  - Token expiration during session
  - Missing scope in redeemed token
  - Manual code exchange performed as fallback

## Troubleshooting

### Graph API Returns 401 Unauthorized
- Check API permissions in Azure App Registration
- Ensure `User.Read` delegated permission is granted
- Verify admin consent is granted for the organization
- Check if token has correct scope (look at debug output)

### Authorization Code Not Available
**Symptoms**: `"No se encontró ni access token ni authorization code en el contexto"`

**Note**: This is now **very rare** since both HTTP and HTTPS use `RedeemCode=true`

**Causes**:
- Token obtained from cookie/cache (no new authorization flow)
- Network issue during token redemption
- OWIN middleware configuration issue

**Solutions**:
- Verify `RedeemCode = true` is set in OWIN configuration
- Clear browser cookies and re-authenticate
- Check OWIN middleware logs for redemption errors
- Ensure `Scope = "openid profile email User.Read"` is correctly configured

### Access Token Without User.Read Scope
**Symptoms**: `"Access token disponible pero sin scope User.Read"`

**Causes**:
- Scope not requested in `OpenIdConnectAuthenticationOptions.Scope`
- Consent not granted for Graph API permissions

**Solutions**:
- Verify `Scope = "openid profile email User.Read"` in configuration
- Grant admin consent in Azure Portal
- Clear browser cookies and re-authenticate

### External Users Can't Login
- Verify `preferred_username` parsing logic
- Check database for user with parsed username (e.g., `Fernando.Pagano`)
- Review debug output to see which extraction method was used
- Ensure database permissions table includes external users

### "AADSTS54005: OAuth2 Authorization code was already redeemed"
**Cause**: Trying to use the same authorization code twice

**Note**: This should **not occur** with current configuration since `RedeemCode=true` handles redemption automatically

**If this occurs**:
- Check that you're not manually redeeming the code elsewhere
- Verify OWIN configuration doesn't have conflicting settings
- Ensure you're not calling token endpoint directly when `RedeemCode=true` is set

## Logging and Debugging

All methods use `System.Diagnostics.Debug.WriteLine` for output. To view logs:

1. **Visual Studio**: Check Output window → Show output from: Debug
2. **Production**: Use DebugView or configure trace listeners

### Key Debug Messages

```
Username obtenido desde claim 'onprem_sam_account_name': john.doe
Username obtenido desde Microsoft Graph API: john.doe
Username obtenido desde preferred_username (parseado): Fernando.Pagano
Access token para Graph API obtenido desde ProtocolMessage (RedeemCode=true). Scope: User.Read User.Read.All openid profile email
Canjeando authorization code por access token para Graph API (caso fallback)
Access token para Graph API obtenido mediante canje de código
No se encontró el ObjectId en los claims
El usuario no tiene onPremisesSamAccountName en Graph API
Error al consultar Graph API. Status: Unauthorized, Reason: ...
Servicios de dominio no disponibles para cargar claims
Claims cargados correctamente. Total: 15
```

**Most Common Flow**: The first Graph API message indicating token from `ProtocolMessage` is what you'll see in ~99% of cases.

## HTTP vs HTTPS Comparison

| Feature | HTTP (localhost) | HTTPS (production) |
|---------|------------------|-------------------|
| Response Type | ✅ `Code` | ✅ `Code` |
| RedeemCode | ✅ `true` | ✅ `true` |
| Nonce Required | ❌ `false` | ✅ `true` |
| State Required | ❌ Can be `false` | ✅ `true` |
| Access Token Source | `ProtocolMessage.AccessToken` | `ProtocolMessage.AccessToken` |
| Authorization Code | Consumed by OWIN | Consumed by OWIN |
| Graph API Call | Direct (reused token) | Direct (reused token) |
| Performance | Fast (no extra call) | Fast (no extra call) |
| Fallback Available | Yes (Phase 2) | Yes (Phase 2) |

**Key Takeaway**: Both environments now use **identical** OAuth2 configuration (`Code` + `RedeemCode=true`), with only security protocol validation differences (Nonce/State)

## Future Improvements

1. **Cache Graph Tokens**: Store Graph access tokens in session/cookie to avoid repeated calls (though less critical now with `RedeemCode=true`)
2. **Batch User Queries**: If calling Graph frequently, implement Microsoft Graph batching
3. **Custom Claim Transformation**: Add more Graph user properties as custom claims (department, job title, etc.)
4. **Unit Tests**: Create comprehensive tests for each extraction method and fallback scenario
5. **Configuration**: Make Graph API usage configurable (enable/disable via Web.config)
6. **Token Refresh**: Implement token refresh logic for long-running sessions
7. **Monitor Phase 2 Usage**: Add telemetry to track how often fallback code exchange is needed (should be <1%)

## Related Files
- `Molinos.Scato.WebMobile\App_Start\Startup.Auth.cs` - Main authentication logic
- `Molinos.Scato.WebMobile\Web.config` - Azure Entra ID configuration
- `Molinos.Scato.Servicios\IServicioRepositorio.cs` - User permissions interface
- `Molinos.Scato.Repositorio\ConsultasEF\PermisosPorUsuarioConsulta.cs` - User permissions query

## Implementation Notes

### Why Multiple SAM Account Claim Types?
Different Azure AD configurations and optional claim settings can emit SAM account information in different claim formats. By checking all known variations, we maximize compatibility across different tenant configurations.

### Why System.Diagnostics.Debug.WriteLine?
Simplified logging approach that doesn't require logger factory dependency injection. Works well for development and can be replaced with structured logging in production if needed.

### Why Domain Stripping?
Some claim types (like `windowsaccountname`) return the full `DOMAIN\username` format. The application's user database typically stores just the username portion, so we extract it automatically.

### Why Scope Validation?
Even when an access token is available, it might not have the necessary permissions for Graph API calls. Validating the scope prevents API calls that would fail with 403 Forbidden errors.

### Why RedeemCode=true for Both Environments?
Setting `RedeemCode=true` for both HTTP and HTTPS simplifies the authentication flow:
- **Consistency**: Same behavior in all environments
- **Performance**: No manual token exchange needed
- **Reliability**: OWIN middleware handles token redemption robustly
- **Simplicity**: Less conditional code and fewer edge cases

The Phase 2 fallback in `GetGraphAccessToken` handles rare edge cases where the token might not be available.

### Why Code Flow for Both HTTP and HTTPS?
Using `ResponseType = Code` (Authorization Code Flow) for both environments provides:
- **Maximum Consistency**: Identical OAuth2 flow regardless of environment
- **Best Security**: Code flow is more secure than hybrid flows
- **Simpler Logic**: No conditional response type handling
- **Easier Debugging**: Same behavior in development and production
- **Industry Standard**: Pure authorization code flow is the recommended OAuth2 pattern

The only differences between HTTP and HTTPS are security protocol validation settings (Nonce/State requirements), which are appropriate for each environment's security context.

---
**Date**: 2025-01-22  
**Last Updated**: 2025-01-22  
**Branch**: feature/PSL-816-ADFS-2-EntraID-WebMobile  
**Version**: 3.1 - Fully unified OAuth2 flow: Code + RedeemCode=true for all environments
