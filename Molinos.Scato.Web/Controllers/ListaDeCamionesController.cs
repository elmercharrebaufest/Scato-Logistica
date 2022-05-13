using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Web.Seguridad;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    public class ListaDeCamionesController : BaseController
    {
        private readonly ILogger log;
        private readonly IListaDeWorkflows workflows;

        public ListaDeCamionesController(ILogger log, IListaDeWorkflows workflows, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.log = log;
            this.workflows = workflows;
        }
        [DatosUsuario]
        public ActionResult Index(Guid? id, string profiling, DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina = 1, string ordenarPor = "FechaUltimaModificacion", DirOrden dirOrden = DirOrden.Desc, bool venimosDeListaDeTareasAutomatizada = false, bool evitarRedireccion = false)
        {
            // Redireccionar a la proxima actividad, si es de inicio automático
            var redirect = evitarRedireccion ? null : RedirectProximaActividad(id, datosUsuario);
            if (redirect != null)
            {
                //Redireccionamos automaticamente a la pantalla de la proxima actividad
                return redirect;
            }

            // Redireccionar a balanza automatizada, si el puesto esta asociado a balanzas automaticas. 
            if (datosUsuario.RedireccionarABalanzaAutomatizada && !venimosDeListaDeTareasAutomatizada)
            {
                return RedirectToAction("Index", "BalanzaAutomatica");
            }
            // De no ser de balanzas automatizadas, Redireccionar a la lista automatizada, si el puesto es automático
            if (datosUsuario.RedireccionarAListaAutomatizada && !venimosDeListaDeTareasAutomatizada)
            {
                return RedirectToAction("Index", "ListaDeTareasAutomatizada");
            }

            ViewBag.VenimosDeListaDeTareasAutomatizada = venimosDeListaDeTareasAutomatizada;

            ////////
            if (profiling != null)
            {
                Response.AppendCookie(new HttpCookie("profiling"));
            }
            /////

            if (filtro.TiempoMaxEntreActividades == null && datosUsuario.CentroId > 0 )
            {
                filtro.TiempoMaxEntreActividades = servicio.ObtenerTiempoMaximoCentro(datosUsuario.CentroId);
            }

            filtro = CrearPrimeraCookie(filtro);
            log.Debug("Cookie Usuario: {0}", new CookieUsuario());

            ViewBag.Workflows = servicio.ListarWorkflowsCodigoPorCentro(datosUsuario.CentroId).OrderBy(x => x.Codigo).ToSelectList(x => x.Codigo, x => x.Descripcion);          
            var actividades = workflows.ObtenerWorkflowProximasAcciones(datosUsuario.NombreUsuario,datosUsuario.CentroId);
            if (PermisosHelper.Is(PermisosScato.CamionesPendientesMesa))
            {
                actividades.Add(PermisosScato.CamionesPendientesMesa.DisplayText());
            }
            if (PermisosHelper.Is(PermisosScato.CamionesPendientesNoGranos))
            {
                actividades.Add(PermisosScato.CamionesPendientesNoGranos.ToString());
            }
            ViewBag.Estados = actividades.ToSelectList(x => x, x => Textos.ResourceManager.GetString("Act" + x));
            ViewBag.MostrarValidacionEtapaAutomatica = TempData["FlagMostrarValidacionEtapaAutomatica"] ?? false;
            ViewBag.MensajeValidacionEtapaAutomatica = TempData["messageEtapaAutomatica"] ?? string.Empty;

            return View(filtro);
        }


        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string refresco, DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina = 1, string ordenarPor = "FechaUltimaModificacion", DirOrden dirOrden = DirOrden.Desc)
        {
            if(string.IsNullOrEmpty(ordenarPor))
            {
                ordenarPor = "FechaUltimaModificacion";
                dirOrden = DirOrden.Desc;
            }
            filtro = CrearCookie(filtro, ordenarPor, dirOrden);

            if (filtro.Patente != null)
            {
                filtro.Patente = filtro.Patente.ToUpper();
            }
            if (filtro.TiempoMaxEntreActividades == null && datosUsuario.CentroId > 0)
            {
                filtro.TiempoMaxEntreActividades = servicio.ObtenerTiempoMaximoCentro(datosUsuario.CentroId);
            }

            ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            if (!string.IsNullOrEmpty(refresco))
            {
                var cookie = new CookieUsuario();
                cookie.ActualizarValor("Refresco", refresco);
            }

            return View("Listar", filtro);
        }

        [DatosUsuario]
        public ActionResult Ejecutar(DatosUsuario datosUsuario, Guid id, string proximaAccion, string codigo)
        {
            if (proximaAccion != "BalanzaACero" && servicio.EsActividadAutomatica(id, proximaAccion, codigo) && !servicio.TienePermiso(datosUsuario.NombreUsuario, PermisosScato.EjecucionManual) && !(proximaAccion == "Calado" && servicio.TienePermiso(datosUsuario.NombreUsuario, PermisosScato.ActividadCaladoRechazar)))
            {
                TempData["Alerta"] = string.Format(Textos.Error_ActividadAutomatica, Textos.ResourceManager.GetString("Act" + proximaAccion));
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            return RedirectToAction("Index", proximaAccion, new { id });
        }

        [DatosUsuario]
        public ActionResult EjecutarPendiente(DatosUsuario datosUsuario, int id, string proximaAccion, string codigo)
        {
            return RedirectToAction("Index", proximaAccion, new { id });
        }
        
        private RedirectToRouteResult RedirectProximaActividad(Guid? id, DatosUsuario datosUsuario)
        {
            var retries = Convert.ToInt32(ConfigurationManager.AppSettings["AutomaticActivityRetries"]);
            var automaticActivityInterval = Convert.ToInt32(ConfigurationManager.AppSettings["AutomaticActivityInterval"]);

            var instanciaWf = id;
            if (!instanciaWf.HasValue)
            {
                if (Request.UrlReferrer != null)
                {
                    Guid guid;
                    if (Guid.TryParse(Request.UrlReferrer.AbsolutePath.Split('/').Last(), out guid))
                    {
                        instanciaWf = guid;
                    }
                }
            }

            if (instanciaWf.HasValue)
            {
                ProximaAccionEjecutableDto proximaAccionEjecutable = null;
                for (int i = 0; i < retries && proximaAccionEjecutable == null; i++)
                {
                    log.Debug("Intentando ejecutar WF {0} automaticamente. Intento {1}", instanciaWf.Value, i);
                    Thread.Sleep(automaticActivityInterval);
                    proximaAccionEjecutable = workflows.ObtenerWorkflowProximaAccionEjecutable(instanciaWf.Value,
                                                                                     datosUsuario.NombreUsuario,
                                                                                     datosUsuario.CentroId);
                    if (proximaAccionEjecutable != null && proximaAccionEjecutable.Ejecutar &&
                        !(Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.Contains("/" + proximaAccionEjecutable.Actividad + "/")))
                    {
                        log.Debug("Ejecutando automaticamente WF {0} actividad {1}...", instanciaWf.Value, proximaAccionEjecutable.Actividad);
                        return RedirectToAction("Index", proximaAccionEjecutable.Actividad, new { id = instanciaWf.Value });
                    }
                }
            }

            return null;
        }

        private void ListQuery(DatosUsuario datosUsuario, FiltroListaDeWorkflowsDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 15);
            filtro.CentroId = datosUsuario.CentroId;
            filtro.NombreUsuario = datosUsuario.NombreUsuario;
            filtro.MostrarCamionesPendientes = PermisosHelper.Is(PermisosScato.CamionesPendientesMesa);
            filtro.MostrarCamionesPendientesNoGranos = PermisosHelper.Is(PermisosScato.CamionesPendientesNoGranos);

            var instancias = workflows.ListarWorkFlows(paginacion, filtro);

            ViewBag.Items = instancias.InstanciasWorkflowDto;
        }

        private FiltroListaDeWorkflowsDto CrearPrimeraCookie(FiltroListaDeWorkflowsDto filtro)
        {
            var cookie = new CookieUsuario();
            if (filtro.Columnas == null)
            {   //carga columnas desde cookie
                filtro.Columnas = new List<string> { cookie.Valor("visibles") };
            }
            else if (filtro.Columnas != null)
            {
                //actualiza cookies con columnas si las actuales son diferentes a las guardadas
                var visibles = filtro.Columnas.Aggregate((a, b) => a + "|" + b);
                if (cookie.Valor("visibles") != visibles)
                {
                    cookie.ActualizarValor("visibles", visibles);
                }
            }
            filtro.Workflow = cookie.Valor("Workflow");
            filtro.Patente = cookie.Valor("Patente");
            filtro.ProximaAccion = cookie.Valor("ProximaAccion");
            filtro.NumeroDocumentoDeIngreso = cookie.Valor("NumeroDocumentoDeIngreso");
            filtro.SoloDemorados = Boolean.Parse(cookie.Valor("SoloDemorados") ?? "false");
            filtro.OrdenarPor = cookie.Valor("OrdenarPor");
            filtro.DirOrden = cookie.Valor("DirOrden") == "1" ?  DirOrden.Desc : DirOrden.Asc;
            filtro.ExcluirRechazados = Boolean.Parse(cookie.Valor("ExcluirRechazados") ?? "false");

            var valorTipoMaterial = cookie.Valor("TipoMaterial");
            TipoMaterial tipoMaterialCookie;
            if (!string.IsNullOrEmpty(valorTipoMaterial) && Enum.TryParse<TipoMaterial>(valorTipoMaterial, out tipoMaterialCookie))
            {
                filtro.TipoMaterial = tipoMaterialCookie;
            } else
            {
                filtro.TipoMaterial = TipoMaterial.Todos;
            }

            var valorTieneEntregador = cookie.Valor("TieneEntregador");
            FiltroEntregador tieneEntregadorCookie;
            if (!string.IsNullOrEmpty(valorTieneEntregador) && Enum.TryParse<FiltroEntregador>(valorTieneEntregador, out tieneEntregadorCookie))
            {
                filtro.TieneEntregador = tieneEntregadorCookie;
            }
            else
            {
                filtro.TieneEntregador = FiltroEntregador.Todos;
            }

            var valor = cookie.Valor("TipoDocumentoDeIngreso");
            TipoDocumentoIngreso tipoDocumentoDeIngresoCookie;
            if (!string.IsNullOrEmpty(valor) && Enum.TryParse<TipoDocumentoIngreso>(valor, out tipoDocumentoDeIngresoCookie))
            {
                filtro.TipoDocumentoDeIngreso = tipoDocumentoDeIngresoCookie;
            }
            return filtro;
        }

        private FiltroListaDeWorkflowsDto CrearCookie(FiltroListaDeWorkflowsDto filtro, string ordenarPor, DirOrden dirOrden)
        {
            var cookie = new CookieUsuario();
            if (filtro.Columnas == null)
            {   //carga columnas desde cookie
                filtro.Columnas = new List<string> { cookie.Valor("visibles") };
            }
            else if (filtro.Columnas != null)
            {
                //actualiza cookies con columnas si las actuales son diferentes a las guardadas
                var visibles = filtro.Columnas.Aggregate((a, b) => a + "|" + b);
                if (cookie.Valor("visibles") != visibles)
                {
                    cookie.ActualizarValor("visibles", visibles);
                }
            }
            cookie.ActualizarValor("Workflow", filtro.Workflow);
            cookie.ActualizarValor("Patente", filtro.Patente);
            cookie.ActualizarValor("TipoDocumentoDeIngreso", filtro.TipoDocumentoDeIngreso.ToString());
            cookie.ActualizarValor("ProximaAccion", filtro.ProximaAccion);
            cookie.ActualizarValor("NumeroDocumentoDeIngreso", filtro.NumeroDocumentoDeIngreso);
            cookie.ActualizarValor("SoloDemorados", filtro.SoloDemorados.ToString());
            cookie.ActualizarValor("OrdenarPor", ordenarPor);
            cookie.ActualizarValor("DirOrden", ((int)dirOrden).ToString());
            cookie.ActualizarValor("ExcluirRechazados", filtro.ExcluirRechazados.ToString());
            cookie.ActualizarValor("TipoMaterial", filtro.TipoMaterial.ToString());
            cookie.ActualizarValor("TieneEntregador", filtro.TieneEntregador.ToString());
            return filtro;
        }
    }
}
