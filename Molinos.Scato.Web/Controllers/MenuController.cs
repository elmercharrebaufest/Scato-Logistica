using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Services;
using System.Linq;
using System.Resources;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;
        private readonly IServicioOrquestador orquestador;

        public MenuController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador orquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
        }

        [DatosUsuario]
        public ActionResult Menu(DatosUsuario datosUsuario)
        {            
            ViewBag.Workflows = servicio.ListarWorkflowsPorUsuarioYCentro(datosUsuario.NombreUsuario, datosUsuario.CentroId);
            ViewBag.Grupos = ObtenerGrupos(datosUsuario);

            #region VisualizacionBarreras
            var puestoTrabajo = servicio.ObtenerPuestoDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId);
            if (puestoTrabajo?.VisualizacionBarrera_Id is null)
            {
                var grupoBarrera = servicio.ObtenerGruposBarrerasPorUsuario(datosUsuario.NombreUsuario, datosUsuario.CentroId);
                for (int i = 0; i < grupoBarrera?.Count; i++)
                {
                    var sensores = servicio.ListarSensoresBarreras(grupoBarrera[i].Id);
                    grupoBarrera[i].SensoresBarreras = sensores;
                }
                ViewBag.ModulosBarrera = grupoBarrera;
                var sensoresBarrera = servicio.ListarSensoresBarreras(grupoBarrera?.FirstOrDefault()?.Id ?? 0);
                ViewBag.SensoresBarrera = sensoresBarrera;
                ViewBag.TotalSensoresBarrera = servicio.ObtenerCantidadBarrerasPorUsuario(datosUsuario.NombreUsuario, datosUsuario.CentroId);
            }
            else
            {
                var configVisualizacionBarrera = servicio.ObtenerVisualizacionBarrera(Convert.ToInt32(puestoTrabajo?.VisualizacionBarrera_Id));
                ViewBag.RequiereComentarioGestionarBarrera = puestoTrabajo.RequiereComentarioGestionarBarrera;
                if (configVisualizacionBarrera != null && !configVisualizacionBarrera.Deshabilitada)
                {
                    configVisualizacionBarrera.SensoresBarreras = servicio.ListarSensoresBarreras(configVisualizacionBarrera.Id);
                    ViewBag.ModulosBarrera = new List<VisualizacionBarreraDto> { configVisualizacionBarrera };
                    ViewBag.SensoresBarrera = configVisualizacionBarrera?.SensoresBarreras;
                    ViewBag.TotalSensoresBarrera = configVisualizacionBarrera.SensoresBarreras?.Count;
                    ViewBag.GestionarBarrera = true;
                }
                else
                {
                    ViewBag.ModulosBarrera = new List<VisualizacionBarreraDto>();
                    ViewBag.SensoresBarrera = new List<SensorBarreraDto>();
                    ViewBag.TotalSensoresBarrera = 0;
                }
            }

            var barreraSupervisor = puestoTrabajo?.EntradaSupervisor?.Split(',').ToList() ?? new List<string>();
            barreraSupervisor.AddRange(puestoTrabajo?.CierreSupervisor?.Split(',').ToList() ?? new List<string>());
            ViewBag.BarrerasSupervisor = barreraSupervisor;
            ViewBag.PuestoId = puestoTrabajo?.Id;
            #endregion

            var rm = new ResourceManager(typeof(Textos));
            ViewBag.Idiomas = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(x => x).Where(x => ResourceManagerExist(rm, x)).ToSelectList(x => x.LCID.ToString(CultureInfo.InvariantCulture), x => x.NativeName.Split('(')[0]);
            ViewBag.Idioma = CultureInfo.CurrentCulture.NativeName.Split('(')[0];
            ViewBag.NombrePc = datosUsuario.NombrePc;
            ViewBag.Mantenimiento = EstaEnMantenimiento();
            return PartialView("_Menu");
        }
        
        public JsonResult MarcarLeidos(int? id)
        {
            if (id != null)
            {
                servicioComandos.Ejecutar(new ModificarNotificacion { Id = id.Value });
            }
            return Json(new object(), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult MarcarLeidosTodos(DatosUsuario datosUsuario)
        {
            servicioComandos.Ejecutar(new ModificarNotificacion { Grupos = ObtenerGrupos(datosUsuario) });
            return Json(new object(), JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerNotificaciones(DatosUsuario datosUsuario, bool listarSobre, bool mostrarAlerta, bool contar)
        {
            var notificaciones = servicio.ObtenerNotificaciones(ObtenerGrupos(datosUsuario), listarSobre, mostrarAlerta, contar);
            return Json(notificaciones, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarNotificacion(int id)
        {
            servicioComandos.Ejecutar(new EliminarNotificacion {Id = id});
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult EliminarNotificaciones(DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarNotificaciones { Grupos = ObtenerGrupos(datosUsuario) }) as ResultadoEliminarNotificaciones;
            return Json(new { grupos = resultado.Grupos }, JsonRequestBehavior.AllowGet);
        }

        private string ObtenerGrupos(DatosUsuario datosUsuario)
        {
            var grupos = datosUsuario.CentroId.ToString();
            grupos = grupos + "," + datosUsuario.CentroId + "|" + datosUsuario.NombreUsuario;
            if (!string.IsNullOrEmpty(datosUsuario.GrupoUsuario))
            {
                grupos = grupos + "," + datosUsuario.GrupoUsuario;
            }
            var puestos = servicio.ObtenerPuestosIdPorPC(datosUsuario.NombrePc);
            foreach (var puesto in puestos)
            {
                grupos = grupos + "," + datosUsuario.CentroId + "|" + puesto;
            }
            return grupos;
        }

        private bool ResourceManagerExist(ResourceManager rm, CultureInfo c)
        {
            try
            {
                if (c.LCID != 127 && (rm.GetResourceSet(c, true, false) != null || c.LCID == 11274))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public ActionResult ChangeCurrentCulture(int lcid)
        {
            //
            // Change the current culture for this user.
            //
            var culture = CultureInfo.GetCultureInfo(lcid);
            SessionManager.CurrentCulture = culture;
            //
            // Cache the new current culture into the user HTTP session. 
            //
            var cookie = new CookieUsuario();
            cookie.ActualizarValor("CurrentCulture", lcid.ToString(CultureInfo.InvariantCulture));
            //
            // Redirect to the same page from where the request was made! 
            //
            return Redirect(Request.UrlReferrer.ToString());
        }

        [AjaxOnly]
        public void BorrarPermisosCookie()
        {
            if (FederatedAuthentication.SessionAuthenticationModule != null )
            {
                FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            }
            
        }

        private bool EstaEnMantenimiento()
        {
            try
            {
                string[] horario = ConfigurationManager.AppSettings["EnMantenimiento"].Split(',');
                var diaConf = Convert.ToInt32(horario[0]);
                var horaConf = Convert.ToInt32(horario[1]);
                var minutoConf = Convert.ToInt32(horario[2]);

                var dia = DateTime.Now.DayOfWeek;
                var hora = DateTime.Now.Hour;
                var minuto = DateTime.Now.Minute;

                if ((int)dia == diaConf
                    && hora >= horaConf
                    && (hora * 60) + minuto <= (horaConf * 60) + minutoConf)
                {
                    ViewBag.Hora = horaConf;
                    ViewBag.Minuto = minutoConf;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                log.Error(e, "Error al obtener EnMantenimiento");
                return false;
            }
        }
        public void ActualizarCookiePermisos()
        {
            var cookie = new CookieUsuario();
            var valorCentro = cookie.Valor("CentroId");
            if (!string.IsNullOrEmpty(valorCentro))
            {
                var nombreUsuario =
                    System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                        System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value;
                var grupos = servicio.ObtenerGruposPorUsuario(nombreUsuario);
                cookie.ActualizarValor("Grupo", String.Join("|", grupos.ToArray()));
            }
        }

        [DatosUsuario]
        public ActionResult GestionarBarrera(DatosUsuario datosUsuario, int puestoId, string codigo, string motivo, string accion)
        {
            try
            {
                switch (accion)
                {
                    case "A": //Apertura de BarreraMaestro
                        orquestador.Ejecutar(new EjecutarAperturaBarreraMaestro { CodigoDispositivo = codigo });
                        break;
                    case "C": //Cierre de Barrera
                        orquestador.Ejecutar(new EjecutarCierreBarrera { CodigoDispositivo = codigo });
                        break;
                    default:
                        break;
                }                
            }
            catch (Exception e)
            {
                log.Error(e, $"Error al {(accion == "A" ? "abrir" : "cerrar")} la barrera : {codigo}");
                return Json($"Error al {(accion == "A" ? "abrir" : "cerrar")} la barrera", JsonRequestBehavior.AllowGet);          
            }
            if (!string.IsNullOrEmpty(motivo))
            {
                servicioComandos.Ejecutar(new CrearLogTarjetaSupervisor { PuestoDeTrabajoId = puestoId, Motivo = $"{motivo} - {(accion == "A" ? "APERTURA" : "CIERRE")} ", Usuario = datosUsuario.NombreUsuario });
            }

            return Json("ok", JsonRequestBehavior.AllowGet);
        }
    }
}

