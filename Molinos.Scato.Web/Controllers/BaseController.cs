using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Models;

namespace Molinos.Scato.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IServicioRepositorio servicio;

        protected BaseController(IServicioRepositorio servicio)
        {
            this.servicio = servicio;
        }

        /// <summary>
        /// Manage the internationalization before to invokes the action in the current controller context.
        /// </summary>
        protected override void ExecuteCore()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            var cookie = new CookieUsuario();
            if (!string.IsNullOrEmpty(cookie.Valor("CurrentCulture")))
            {
                culture = CultureInfo.GetCultureInfo(int.Parse(cookie.Valor("CurrentCulture")));
            }
            SessionManager.CurrentCulture = culture;
            //
            // Invokes the action in the current controller context.
            //

            CargarDatosCookie();

            base.ExecuteCore();
        }

        protected void CargarDatosCookie()
        {
            var cookie = new CookieUsuario();            
            var valorCentro = cookie.Valor("CentroId");
            if (string.IsNullOrEmpty(valorCentro) || int.Parse(valorCentro) == 0)
            {
                var nombreUsuario =
                    System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                        System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value;
                var centros = servicio.ListarCentrosPorUsuario(nombreUsuario);
                if (centros != null && centros.Count >= 1)
                {
                    var centro = centros[0];
                    var balanzas = servicio.ListarBalanzasActivas(centro.Id, TipoVehiculo.Camión);
                    cookie.ActualizarValor("CentroId", centro.Id.ToString(CultureInfo.InvariantCulture));
                    cookie.ActualizarValor("CentroDescripcion", centro.Descripcion);
                    cookie.ActualizarValor("CentroCodigoSap", centro.CodigoSAP);

                    var balanzaId = balanzas.Count > 0 ? balanzas[0].Id : 0;
                    cookie.ActualizarValor("BalanzaId", balanzaId.ToString(CultureInfo.InvariantCulture));

                    var grupos = servicio.ObtenerGruposPorUsuario(nombreUsuario);
                    cookie.ActualizarValor("Grupo", String.Join("|", grupos.ToArray()));
                }
            }
        }

        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }

        public Uri GetVirtualAddress(string codigoWorkflow)
        {
            if (HttpContext.Request == null || HttpContext.Request.Url == null || HttpContext.Request.ApplicationPath == null)
            {
                throw new InvalidOperationException();
            }
            var urib = new UriBuilder(HttpContext.Request.Url) { Path = VirtualPathUtility.ToAbsolute(string.Format("~/xaml/{0}.xamlx", codigoWorkflow)), Query = string.Empty };
            return urib.Uri;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            //If the exeption is already handled we do nothing
            var faultException = filterContext.Exception as System.ServiceModel.FaultException;
            if (!filterContext.ExceptionHandled && faultException != null && faultException.Code.Name == "OperationNotAvailable")
            {
                var controllerType = filterContext.Controller.GetType();

                TempData["Alerta"] = String.Format(Textos.Error_ActividadYaEjecutada, Textos.ResourceManager.GetString("Act" + controllerType.Name.Replace("Controller","")));
                TempData["TipoAlerta"] = TipoAlerta.Error;
                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new ContentResult { Content = "ErrorActividadYaEjecutada" };
                }
                else
                {
                    filterContext.Result = RedirectToAction("Index", "ListaDeCamiones");
                }
   
                
                //Make sure that we mark the exception as handled
                filterContext.ExceptionHandled = true;
            }
            
        }
    }
}