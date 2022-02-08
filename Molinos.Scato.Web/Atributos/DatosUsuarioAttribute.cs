using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Web.Models;

namespace Molinos.Scato.Web.Atributos
{
    public class DatosUsuarioAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            const string key = "datosUsuario";
            var cookie = new CookieUsuario();
            var datosUsuario = new DatosUsuario();
            var NombrePc = HttpContext.Current.Request.Cookies["NombrePc"] != null ?HttpContext.Current.Request.Cookies["NombrePc"].Value : "NoTienePuesto";
            datosUsuario.NombreUsuario = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value;
            datosUsuario.Grupo = cookie.Valor("Grupo").Split('|');
            datosUsuario.CentroId = int.Parse(cookie.Valor("CentroId"));
            datosUsuario.CentroDescripcion = cookie.Valor("CentroDescripcion");
            datosUsuario.CentroCodigoSap = cookie.Valor("CentroCodigoSap");
            datosUsuario.BalanzaId = int.Parse(cookie.Valor("BalanzaId"));
            var puestoId = HttpContext.Current.Request.Cookies["PuestoDeTrabajoId"] != null ? HttpContext.Current.Request.Cookies["PuestoDeTrabajoId"].Value : "0";
            datosUsuario.PuestoDeTrabajoId = int.Parse(puestoId);
            datosUsuario.NombrePc =System.Security.Claims.ClaimsPrincipal.Current.FindFirst(x => x.Type == "UserComputerName")?.Value ?? NombrePc;         
            filterContext.ActionParameters[key] = datosUsuario;
            base.OnActionExecuting(filterContext);
            var redireccionar = HttpContext.Current.Request.Cookies["RedireccionarAListaAutomatizada"] != null ? HttpContext.Current.Request.Cookies["RedireccionarAListaAutomatizada"].Value : "false";
            datosUsuario.RedireccionarAListaAutomatizada = redireccionar == "true";

            var redirecBalanza = HttpContext.Current.Request.Cookies["RedireccionarABalanzaAutomatizada"] != null ? HttpContext.Current.Request.Cookies["RedireccionarABalanzaAutomatizada"].Value : "false";
            datosUsuario.RedireccionarABalanzaAutomatizada = redirecBalanza == "true";
        }
    }
}
