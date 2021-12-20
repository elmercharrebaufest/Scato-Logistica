using System;
using System.Security.Authentication;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Seguridad;

namespace Molinos.Scato.WebOperaciones.Atributos
{
    public sealed class AutorizacionAttribute : AuthorizeAttribute
    {
        public AutorizacionAttribute(params PermisosScato[] permisos)
        {
            base.Roles = string.Join(", ", permisos);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations"), Obsolete("Usar el constructor con la lista de PermisosScato.", true)]
        public new string Roles
        {
            get { throw new AuthenticationException("No usar esta propiedad. Usar el constructor con la lista de PermisosScato."); }
            set { throw new AuthenticationException("No usar esta propiedad. Usar el constructor con la lista de PermisosScato."); }
        }
    }
}
