using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Consultas;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.PanelDeControlTransaccionesSap)]
    public class PanelDeControlPagoMunicipalController : BaseController
    {

        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly WaybillManagementPODv2 servicioMonsanto;
        private readonly IServicioSapAsincronico servicioSapAsinc;

        public PanelDeControlPagoMunicipalController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, ZSDWS_SCATO servicioSap, WaybillManagementPODv2 servicioMonsanto, IServicioSapAsincronico servicioSapAsinc)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioSap = servicioSap;
            this.servicioMonsanto = servicioMonsanto;
            this.servicioSapAsinc = servicioSapAsinc;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc, TipoDeServicio tipoDeServicio = TipoDeServicio.Sap)
        {
            var filtro = SetearFiltro();
            filtro.TipoDeServicio = tipoDeServicio;

            if (ModelState.IsValid)
            {
                ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            }
            else
            {
                ViewBag.Items = new ListaPaginada<TransmisionASapDto>(new List<TransmisionASapDto>(), 1, 1, 0);
            }
            return View(filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, FiltroPanelPagoMunicipalDto filtro, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            if (ModelState.IsValid)
            {
                PersistirFiltro(filtro);
                ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            }
            else
            {
                ViewBag.Items = new ListaPaginada<PanelPagoMunicipalDto>(new List<PanelPagoMunicipalDto>(), 1, 1, 0);
            }
            return View("Listar", filtro);
        }

        private void ListQuery(DatosUsuario datosUsuario, FiltroPanelPagoMunicipalDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            if (filtro.Patente != null)
            {
                filtro.Patente = filtro.Patente.ToUpper();
            }

            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            var resultado = servicioComandos.Ejecutar(new ConsultarPanelPagoMunicipal { Filtro = filtro, Paginacion = paginacion }) as ResultadoConsultarPanelPagoMunicipal;
            ViewBag.Items = resultado != null && !resultado.HayErrores
                ? resultado.ListaResultados
                : new ListaPaginada<PanelPagoMunicipalDto>(new List<PanelPagoMunicipalDto>(), 1, 1, 0);
            ViewBag.Workflows = servicio.ListarWorkflowsCodigoPorCentro(datosUsuario.CentroId).OrderBy(x => x.Descripcion).ToSelectList(x => x.Codigo.ToString(), x => x.Descripcion);
        }

        [DatosUsuario]
        public ActionResult Reprocesar(DatosUsuario datosUsuario, string registrosSeleccionados)
        {
            var ids = registrosSeleccionados.Split('|');
            log.Debug("Iniciando reprocesar pagos tasa municipal");
           
            var resultado = new List<TipoAlerta>();
            string content;
            string errores = string.Empty;
            foreach (var id in ids)
            {
                var resutadoReprocesar = servicioComandos.Ejecutar(new ReprocesarPagoTasaMunicipal { Id = Convert.ToInt32(id), PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId }) as ResultadoReprocesarPagoTasaMunicipal;

                if (resutadoReprocesar != null)
                {
                    if(resutadoReprocesar.HayErrores)
                    {
                        resultado.Add(TipoAlerta.Error);
                        errores = string.Join(",", resutadoReprocesar.Errores.Values);
                    }     
                    else
                        resultado.Add(TipoAlerta.Exito);
                }
                else
                {
                    resultado.Add(TipoAlerta.Error);
                }
            }
           
            if (resultado.Count(x => x == TipoAlerta.Exito) == ids.Length)
            {
                content = "OK";
            }
            else
            {
                content = "E-" + errores;
            }
            return new ContentResult { Content = content };
        }

        private FiltroPanelPagoMunicipalDto SetearFiltro()
        {
            var filtro = new FiltroPanelPagoMunicipalDto();

            // Inicialización por defecto
            filtro.IngresoDesde = DateTime.Now.Date;
            filtro.IngresoHasta = DateTime.Now.Date;
            filtro.NroDocumento = string.Empty;
            filtro.Patente = string.Empty;
            filtro.Workflow = string.Empty;
            filtro.PagoConsumido = null;

            if (System.Web.HttpContext.Current != null)
            {
                var fechaDesde = System.Web.HttpContext.Current.Request.Cookies["fechaDesde"];
                var fechaHasta = System.Web.HttpContext.Current.Request.Cookies["fechaHasta"];
                var estado = System.Web.HttpContext.Current.Request.Cookies["estado"];
                var nroDocumento = System.Web.HttpContext.Current.Request.Cookies["nroDocumento"];
                var patente = System.Web.HttpContext.Current.Request.Cookies["patente"];
                var workflow = System.Web.HttpContext.Current.Request.Cookies["workflow"];
                var pagoConsumido = System.Web.HttpContext.Current.Request.Cookies["pagoConsumido"];

                if (fechaDesde != null)
                    filtro.IngresoDesde = DateTime.Parse(fechaDesde.Value);

                if (fechaHasta != null)
                    filtro.IngresoHasta = DateTime.Parse(fechaHasta.Value);

                if (nroDocumento != null)
                    filtro.NroDocumento = nroDocumento.Value;

                if (patente != null)
                    filtro.Patente = patente.Value;

                if (workflow != null)
                    filtro.Workflow = workflow.Value;

                if (pagoConsumido != null)
                    filtro.PagoConsumido = string.IsNullOrEmpty(pagoConsumido.Value) ? (bool?)null : bool.Parse(pagoConsumido.Value);
            }

            return filtro;
        }

        private void PersistirFiltro(FiltroPanelPagoMunicipalDto filtro)
        {
            if (System.Web.HttpContext.Current != null)
            {
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("fechaDesde", filtro.IngresoDesde.ToString("yyyy-MM-dd")));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("fechaHasta", filtro.IngresoHasta.ToString("yyyy-MM-dd")));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("nroDocumento", filtro.NroDocumento ?? string.Empty));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("patente", filtro.Patente ?? string.Empty));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("workflow", filtro.Workflow ?? string.Empty));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("pagoConsumido", filtro.PagoConsumido.HasValue ? filtro.PagoConsumido.Value.ToString() : string.Empty));
            }
        }

    }
}
