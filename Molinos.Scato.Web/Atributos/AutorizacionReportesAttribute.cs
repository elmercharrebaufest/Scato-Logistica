using System;
using System.Linq;
using System.Security.Authentication;
using System.Web.Mvc;
using Molinos.Scato.Servicios;
using Ninject;

namespace Molinos.Scato.Web.Atributos
{
    public sealed class AutorizacionReportesAttribute : AuthorizeAttribute
    {
        [Inject]
        public IServicioRepositorio Servicio { set; private get; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var nombreReporte = filterContext.HttpContext.Request.Params["nombreReporte"];
            var permisos = Servicio.ListarPermisosPorActividad(nombreReporte);
            base.Roles = string.Join(", ", permisos.Select(x => x.Codigo));
            base.OnAuthorization(filterContext);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations"), Obsolete("Usar el constructor con la lista de PermisosScato.", true)]
        public new string Roles
        {
            get { throw new AuthenticationException("No usar esta propiedad. Usar el constructor con la lista de PermisosScato."); }
            set { throw new AuthenticationException("No usar esta propiedad. Usar el constructor con la lista de PermisosScato."); }
        }
    }
}
