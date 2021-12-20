using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.WebPuertoApi.Seguridad;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Molinos.Scato.WebPuertoApi.Atributos
{
    public sealed class AutorizacionAttribute : AuthorizeAttribute
    {
        private PermisosScato[] permisos;
        public AutorizacionAttribute(params PermisosScato[] permisos)
        {
            this.permisos = permisos;
            base.Roles = string.Join(", ", permisos);
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return PermisosHelper.Is(permisos);
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!PermisosHelper.Is(permisos))
            {
                HandleUnauthorizedRequest(actionContext);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations"), Obsolete("Usar el constructor con la lista de PermisosScato.", true)]
        public new string Roles
        {
            get { throw new AuthenticationException("No usar esta propiedad. Usar el constructor con la lista de PermisosScato."); }
            set { throw new AuthenticationException("No usar esta propiedad. Usar el constructor con la lista de PermisosScato."); }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("No tiene permisos suficientes para realizar la acción") };
        }
    }
}