using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.PanelDeControlTransaccionesSap)]
    public class PanelDeControlTransaccionesSapController : BaseController
    {

        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly WaybillManagementPODv2 servicioMonsanto;
        private readonly IServicioSapAsincronico servicioSapAsinc;

        public PanelDeControlTransaccionesSapController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, ZSDWS_SCATO servicioSap, WaybillManagementPODv2 servicioMonsanto, IServicioSapAsincronico servicioSapAsinc)
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
        public ActionResult Listar(DatosUsuario datosUsuario, FiltroPanelDeTransaccionesSapDto filtro, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            if (ModelState.IsValid)
            {
                PersistirFiltro(filtro);
                ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            }
            else
            {
                ViewBag.Items = new ListaPaginada<TransmisionASapDto>(new List<TransmisionASapDto>(), 1, 1, 0);
            }
            return View("Listar", filtro);
        }

        private void ListQuery(DatosUsuario datosUsuario, FiltroPanelDeTransaccionesSapDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            if (filtro.Patente != null)
            {
                filtro.Patente = filtro.Patente.ToUpper();
            }
            filtro.CentroId = datosUsuario.CentroId;

            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarTransmisionesASap(filtro, paginacion);
        }

        public ActionResult Transmitir(string transmisiones)
        {
            var ids = transmisiones.Split('|');
            log.Debug("Iniciando transmisiones SAP");
            var resultado = new List<TipoAlerta>();
            foreach (var id in ids)
            {
                var r = servicioComandos.Ejecutar(new RetransmitirTransmisionASap { Id = Convert.ToInt32(id) }) as ResultadoRetransmitirTransmisionASap;

                if (r != null)
                {
                    resultado.Add(r.Resultado);
                }
                else
                {
                    resultado.Add(TipoAlerta.Error);
                }
            }
            string content;
            if (resultado.Count(x => x == TipoAlerta.Exito) == ids.Length)
            {
                content = "OK";
            }
            else if (resultado.Count(x => x == TipoAlerta.Exito) > 0)
            {
                content = "W-" + Textos.PanelDeControlTransSAP__MensajeAdvertencia;
            }
            else
            {
                content = "E-" + Textos.PanelDeControlTransSAP__MensajeError;
            }
            return new ContentResult { Content = content };
        }

        public ActionResult NoTransmitir(string transmisiones)
        {
            var ids = transmisiones.Split('|');

            foreach (var id in ids)
            {
                var transmision = servicio.ObtenerTransmisionASap(Convert.ToInt32(id));
                transmision.Estado = EstadoTransmisionASap.Cancelada; //POR DEFECTO ASIGNO ERROR
                transmision.Fecha = DateTime.Now;
                transmision.Id = Convert.ToInt32(id);
                servicioComandos.Ejecutar(new ActualizarTransmisionASap { Dto = transmision });
            }

            return new ContentResult { Content = "OK" };
        }

        [Autorizacion(PermisosScato.PanelDeControlTransaccionesSapCupo)]
        public ActionResult RetransmitirTodas(string listaIds)              //Solo Para Cupos
        {
            var filtro = listaIds.FromJson<FiltroPanelDeTransaccionesSapDto>();
            var transmisiones = servicio.ListarCuposEnErrorASapPorFiltro(filtro);
            var numeroTransmisiones = transmisiones.Count();
            if (numeroTransmisiones == 0)
            {
                TempData["Alerta"] = Textos.RetransmitirTodas_Error;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index");
            }
            foreach (var transmision in transmisiones)
            {
                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("Retrasmitidas", numeroTransmisiones.ToString(CultureInfo.InvariantCulture)));
                }
                log.Info("Se inicia transmisión asincrónica con Id: {0}", transmision.InstanciaWorkflow);
                servicioSapAsinc.InformarCupo(transmision.InstanciaWorkflow, transmision.Z2200);
                numeroTransmisiones--;
            }
            return new ContentResult { Content = "OK" };
        }

        [Autorizacion(PermisosScato.AbmTransmisionASap)]
        [DatosUsuario]
        public ActionResult Modificar(int id, DatosUsuario datosUsuario)
        {
            var transmisionTipoDto = servicio.ObtenerTransmisionASapPorIdyFuncionSap(id);
            if (transmisionTipoDto.EstadoTransmision != EstadoTransmisionASap.Error)
            {
                TempData["Alerta"] = Textos.ModificarTransmisionSap_Error;
                TempData["TipoAlerta"] = TipoAlerta.Error;
                return RedirectToAction("Index");
            }
            log.Info("Se solicitó modificar la transmisión a sap con Id: {0}, por el usuario:{1}", id, datosUsuario.NombreUsuario);
            return View(transmisionTipoDto);

        }

        [HttpPost]
        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmTransmisionASap)]
        public ActionResult Modificar(DatosUsuario datosUsuario, TransmisionSapAModificarDto model)
        {
            foreach (var campo in model.Campos)
            {
                if (campo.Value.Length > 100)
                {
                    ModelState.AddModelError(campo.Key, string.Format(Textos.Error_ExcedeLargoMaximo, campo.Key));
                }
            }
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ActualizarTransmision { Dto = model });
                if (!resultado.HayErrores)
                {
                    log.Info("Se modificó la transmisión a sap con Id: {0}, por el usuario:{1}", model.Id, datosUsuario.NombreUsuario);
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        private FiltroPanelDeTransaccionesSapDto SetearFiltro()
        {
            var filtro = new FiltroPanelDeTransaccionesSapDto();
            filtro.FechaDesde = DateTime.Now.Date;
            filtro.FechaHasta = DateTime.Now.Date;
            filtro.EstadoTransmisionASap = EstadoTransmisionASap.Error;
            if (System.Web.HttpContext.Current != null)
            {
                var fechaDesde = System.Web.HttpContext.Current.Request.Cookies["fechaDesde"];
                var fechaHasta = System.Web.HttpContext.Current.Request.Cookies["fechaHasta"];
                var horaDesde = System.Web.HttpContext.Current.Request.Cookies["horaDesde"];
                var horaHasta = System.Web.HttpContext.Current.Request.Cookies["horaHasta"];
                var estado = System.Web.HttpContext.Current.Request.Cookies["estado"];
                var nroDocumento = System.Web.HttpContext.Current.Request.Cookies["nroDocumento"];
                var patente = System.Web.HttpContext.Current.Request.Cookies["patente"];
                var tipoDoc = System.Web.HttpContext.Current.Request.Cookies["tipoDoc"];

                if (fechaDesde != null && fechaHasta != null && estado != null && nroDocumento != null && patente != null && tipoDoc != null && horaDesde != null && horaHasta != null)
                {
                    filtro.FechaDesde = DateTime.Parse(fechaDesde.Value);
                    filtro.FechaHasta = DateTime.Parse(fechaHasta.Value);
                    filtro.HoraDesde = horaDesde.Value;
                    filtro.HoraHasta = horaHasta.Value;
                    filtro.EstadoTransmisionASap = string.IsNullOrEmpty(estado.Value) ? (EstadoTransmisionASap?)null : (EstadoTransmisionASap)Enum.Parse(typeof(EstadoTransmisionASap), estado.Value);
                    filtro.NumeroDocumentoIngreso = nroDocumento.Value;
                    filtro.Patente = patente.Value;
                    filtro.TipoDocumentoIngreso = string.IsNullOrEmpty(tipoDoc.Value) ? (TipoDocumentoIngreso?)null : (TipoDocumentoIngreso)Enum.Parse(typeof(TipoDocumentoIngreso), tipoDoc.Value);
                }
            }

            return filtro;
        }

        private void PersistirFiltro(FiltroPanelDeTransaccionesSapDto filtro)
        {
            if (System.Web.HttpContext.Current != null)
            {
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("fechaDesde", filtro.FechaDesde.ToString()));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("fechaHasta", filtro.FechaHasta.ToString()));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("horaDesde", filtro.HoraDesde));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("horaHasta", filtro.HoraHasta));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("estado", filtro.EstadoTransmisionASap.ToString()));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("nroDocumento", filtro.NumeroDocumentoIngreso));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("patente", filtro.Patente));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("tipoDoc", filtro.TipoDocumentoIngreso.ToString()));
            }
        }

        [AllowAnonymous]
        public ActionResult TransmitirCuposRechazados()
        {
            var resultado = servicioComandos.Ejecutar(new EnviarZE7550 { });
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

    }
}
