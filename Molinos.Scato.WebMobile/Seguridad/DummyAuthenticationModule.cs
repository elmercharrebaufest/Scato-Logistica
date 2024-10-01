using Molinos.Scato.Dominio.Seguridad;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;

namespace Molinos.Scato.WebMobile.Seguridad
{
    public class DummyAuthenticationModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnAuthenticateRequest;
        }

        private void OnAuthenticateRequest(object sender, System.EventArgs e)
        {
            var context = HttpContext.Current;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "molinosagro\\lavrench"),
                new Claim(ClaimTypes.NameIdentifier, "molinosagro\\lavrench"),
                new Claim("CentroDescripcion", "SLO"),
                new Claim("CentroId", "5"),
                new Claim("EstacionMeteorologica", "noConfigurada")
            };

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