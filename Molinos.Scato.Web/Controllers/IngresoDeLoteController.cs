using System;
using System.Globalization;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadIngresoDeLote)]
    public class IngresoDeLoteController : BaseController
    {
        private readonly IServicioActividadFactory<IIngresoDeLoteService> factory;
        private readonly IServicioComandos servicioComandos;
        private readonly ZSDWS_SCATO servicioSap;
        private ILogger log;

        public IngresoDeLoteController(ILogger log, IServicioActividadFactory<IIngresoDeLoteService> factory, IServicioRepositorio servicio, IServicioComandos servicioComandos, ZSDWS_SCATO servicioSap)
            : base(servicio)
        {
            this.factory = factory;
            this.servicioComandos = servicioComandos;
            this.servicioSap = servicioSap;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.Almacenes = servicio.ListarAlmacenesPorCentroYesSustentable(recorrido.Centro.Id,recorrido.EsSustentable).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            return View(new IngresoDeLoteModel { InstanciaWorkflow = recorrido.InstanciaWorkflow, WorkflowDefinicionId = recorrido.WorkflowDefinicionId, Kgs = recorrido.PesoNeto,  Material = recorrido.Material.CodigoSAP, Centro = recorrido.Centro.CodigoSAP, AlmacenId = recorrido.Almacen.Id.ToString(CultureInfo.InvariantCulture)});
        }

        [DatosUsuario]
        public ActionResult Aceptar(IngresoDeLoteModel loteModel, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var loteService = factory.CrearServicio(loteModel.WorkflowDefinicionId);
                    var almacen = servicio.ObtenerAlmacen(Convert.ToInt32(loteModel.AlmacenId));
                    var validaStockYLote = new ValidaStockYLote
                    {
                        Centro = loteModel.Centro,
                        Material = loteModel.Material,
                        Almacen = almacen.CodigoSAP,
                        Lote = loteModel.NroLote,
                        Stock = loteModel.Kgs.HasValue ? loteModel.Kgs.ToString() : "0"
                    };
                    var datosRequest = new ValidaStockYLoteRequest
                    {
                        ValidaStockYLote = validaStockYLote
                    };
                    var respuestaConsultaOrdenCarga = servicioSap.ValidaStockYLote(datosRequest);
                    if (respuestaConsultaOrdenCarga.ValidaStockYLoteResponse.Resultado.CODIGO == 0)
                    {
                        var controlRecorrido = new ControlRecorridoDto
                            {
                                Actividad = Textos.ActIngresoDeLote,
                                ActividadXaml = "IngresoDeLote",
                                WorkflowInstanceId = loteModel.InstanciaWorkflow,
                                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                                NombreUsuario = datosUsuario.NombreUsuario
                            };
                        GuardarLote(validaStockYLote, loteModel.InstanciaWorkflow);
                        loteService.IngresoDeLote(loteModel.InstanciaWorkflow, loteModel.NroLote, Convert.ToInt32(loteModel.AlmacenId), controlRecorrido);
                        return Json("OK", JsonRequestBehavior.AllowGet);
                    }
                    loteModel.Mensaje = respuestaConsultaOrdenCarga.ValidaStockYLoteResponse.Resultado.CODIGO == 1
                                            ? Textos.IngresarLote_SinStock
                                            : Textos.IngresarLote_LoteInexistente;
                    return View("Confirmacion", loteModel);
                }
                catch
                {
                    loteModel.Mensaje = Textos.IngresarLote_ErrorSap;
                    return View("Confirmacion", loteModel);
                }
            }
            TempData["Alerta"] = Textos.IngresarLote_LoteObligatorio;
            TempData["TipoAlerta"] = TipoAlerta.Error;
            return Json("ERROR", JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult Confirmar(IngresoDeLoteModel loteModel, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
        {
                Actividad = Textos.ActIngresoDeLote,
                ActividadXaml = "IngresoDeLote",
                WorkflowInstanceId = loteModel.InstanciaWorkflow,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };
            GuardarLote(new ValidaStockYLote { Lote = loteModel.NroLote }, loteModel.InstanciaWorkflow);
            var loteService = factory.CrearServicio(loteModel.WorkflowDefinicionId);
            loteService.IngresoDeLote(loteModel.InstanciaWorkflow, loteModel.NroLote, Convert.ToInt32(loteModel.AlmacenId), controlRecorrido);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        private void GuardarLote(ValidaStockYLote validaStockYLote, Guid instanciaWorkflow)
        {
            try
            {
                servicioComandos.Ejecutar(new CrearLoteDeRedespacho
                {
                    Dto = new LoteDeRedespachoDto
                    {
                        Almacen = validaStockYLote.Almacen,
                        Centro = validaStockYLote.Centro,
                        Lote = validaStockYLote.Lote,
                        Material = validaStockYLote.Material,
                        Stock = validaStockYLote.Stock,
                        InstanciaWorkflow = instanciaWorkflow
                    }
                });
            }
            catch (Exception e)
            {
                log.Error(e, "Error al crear LoteDeRedespachoDto");
            }
        }
    }
}
