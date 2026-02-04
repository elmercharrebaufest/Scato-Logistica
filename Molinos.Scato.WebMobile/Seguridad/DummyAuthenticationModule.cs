using Molinos.Scato.Dominio.Seguridad;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;

namespace Molinos.Scato.WebMobile.Seguridad
{
    /// <summary>
    /// Development authentication module that bypasses Azure Entra ID authentication
    /// This module provides fake claims for testing purposes without requiring actual authentication
    /// Comment out in Web.config when testing with real Entra ID authentication
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

            // Create claims compatible with Azure Entra ID structure
            var claims = new List<Claim>
            {
                // Standard identity claims
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.NameIdentifier, "molinosagro\\lavrench"),
                
                // Entra ID-style claims
                new Claim("preferred_username", "lavrench@molinosagro.com"),
                new Claim(ClaimTypes.Email, "lavrench@molinosagro.com"),
                new Claim(ClaimTypes.Upn, "lavrench@molinosagro.com"),
                
                // Application-specific claims (these are added by Startup.Auth.cs in production)
                new Claim("CentroDescripcion", "SLO"),
                new Claim("CentroId", "5"),
                new Claim("EstacionMeteorologica", "noConfigurada")
            };

            // Add all permissions for development (mimics database permission loading)
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
