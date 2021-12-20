using System.Linq;
using System.Security.Claims;
using Molinos.Scato.Dominio.Seguridad;

namespace Molinos.Scato.WebOperaciones.Seguridad
{
    public class PermisosHelper
    {
        public static bool Is(params PermisosScato[] permisos)
        {
            var permisosUsuario = ClaimsPrincipal.Current.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
            return permisos.Any(p => permisosUsuario.Contains(p.ToString()));
        }
    }
}