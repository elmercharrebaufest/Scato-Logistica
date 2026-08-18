using Molinos.Scato.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject.Extensions.Logging;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Dominio.Consultas;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.DevolucionDeReciboMunicipal)]
    public class DevolucionDeReciboMunicipalController : BaseController
    {
        private readonly ILogger logger;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioActividadFactory<IEjecutarService> factory;
        private readonly IListaDeWorkflows workflows;

        public DevolucionDeReciboMunicipalController(IServicioRepositorio servicio, ILogger logger, IServicioComandos servicioComandos, IServicioActividadFactory<IEjecutarService> factory, IListaDeWorkflows workflows)
            : base(servicio)
        {
            this.logger = logger;
            this.factory = factory;
            this.servicioComandos = servicioComandos;
            this.workflows = workflows;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            var puestoDeTrabajo = servicio.ListarPuestosDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId).Where(x => x.PidePatente).FirstOrDefault();

            ViewBag.PuestoDeTrabajoId = puestoDeTrabajo != null ? puestoDeTrabajo.Id : 0;
            ViewBag.Garita = puestoDeTrabajo != null ? puestoDeTrabajo.NombreGarita  : "";
            ViewBag.CentroId = datosUsuario.CentroId;

            return View();
        }

        [HttpPost]
        public ActionResult DevolverConMercadoPago(ValoresPagarConMercadoPagoDto valoresDeEntrada)
        {
            var resultadoPago = new ResultadoPagarMercadoPago();
            try
            {
                logger.Debug($"DevolverConMercadoPago {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId}");
                ValidarTarjeta(valoresDeEntrada, resultadoPago);
                var pago = ValidarPago(valoresDeEntrada, resultadoPago);

                if (!resultadoPago.HayErrores)
                {
                    logger.Debug($"DevolverConMercadoPago - procesando pago - {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId}");
                    if (pago.Reembolsable)
                    {
                        resultadoPago = (ResultadoPagarMercadoPago)servicioComandos.Ejecutar(new DevolverMercadoPago { dto = valoresDeEntrada });
                    }
                    else
                    {
                        resultadoPago.Errores.Add("", string.Format(Textos.Error_Reembolso, valoresDeEntrada.NumeroDeTarjeta));
                    }
                }
                else
                {
                    logger.Debug($"DevolverConMercadoPago - tarjeta invalida - {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId}");
                }
            }
            catch (Exception e)
            {
                resultadoPago.Errores.Add("", "Hubo un error al Procesar el pago.");
            }
            return Json(resultadoPago);
        }
        
        [HttpPost]
        public ActionResult PagarConMercadoPago(ValoresPagarConMercadoPagoDto valoresDeEntrada)
        {
            var resultadoPago = new ResultadoPagarMercadoPago();
            try
            {
                logger.Debug($"PagarConMercadoPago {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId}");
                ValidarTarjeta(valoresDeEntrada, resultadoPago);
                if (!resultadoPago.HayErrores)
                {
                    logger.Debug($"PagarConMercadoPago - procesando pago - {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId}");
                    resultadoPago = (ResultadoPagarMercadoPago)servicioComandos.Ejecutar(new PagarMercadoPago { dto = valoresDeEntrada });

                }
                else
                {
                    logger.Debug($"PagarConMercadoPago - tarjeta invalida - {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId}");
                }
            }
            catch (Exception e)
            {
                resultadoPago.Errores.Add("", "Hubo un error al Procesar el pago.");
            }
            return Json(resultadoPago);
        }

        [HttpPost]
        public ActionResult PagarConEfectivo(ValoresPagarConMercadoPagoDto valoresDeEntrada)
        {
            var resultadoPago = new ResultadoPagarMercadoPago();
            
            return Json(resultadoPago);
        }
        
        public ActionResult ObtenerDatos(string numeroDeTarjeta, int puestodetrabajoId)
        {
            var resultadoPago = new ResultadoPagarMercadoPago();

            ValidarTarjeta(new ValoresPagarConMercadoPagoDto { NumeroDeTarjeta = numeroDeTarjeta, PuestoDeTrabajoId = puestodetrabajoId }, resultadoPago);
            var pago = ValidarPago(new ValoresPagarConMercadoPagoDto { NumeroDeTarjeta = numeroDeTarjeta, PuestoDeTrabajoId = puestodetrabajoId }, resultadoPago);
            if (resultadoPago.HayErrores)
            {
                return Json(new { HayErrores = true, Error = resultadoPago.Errores.First() }, JsonRequestBehavior.AllowGet);
            }
            var reciboMunicipal = servicio.ObtenerRecorridoImpresionReciboMunicipalPorTarjeta(numeroDeTarjeta);
            reciboMunicipal.Devuelto = pago.Devuelto || !pago.Reembolsable;
            return Json(reciboMunicipal, JsonRequestBehavior.AllowGet);
        }

        private void ValidarTarjeta(ValoresPagarConMercadoPagoDto valoresDeEntrada, ResultadoPagarMercadoPago resultadoPago)
        {
            var recorrido = servicio.ObtenerDatosRecorridoActivo(null, valoresDeEntrada.NumeroDeTarjeta);
            if (recorrido == null)
            {
                var mensaje = "Error al obtener el vehículo por tarjeta: " + valoresDeEntrada.NumeroDeTarjeta;
                logger.Warn(mensaje);
                resultadoPago.Errores.Add("", mensaje);
                return;
            }
            
            var proximaActividad = workflows.ObtenerWorkflowProximaAccion(recorrido.InstanciaWorkflow);
            if (!String.IsNullOrEmpty(proximaActividad.Mensaje))
            {
                var mensaje = "Error al obtener la proxima accion: " + proximaActividad.Mensaje;
                logger.Warn(mensaje);
                resultadoPago.Errores.Add("", mensaje);
                return;
            }
        }

        private PagoConMercadoPagoDto ValidarPago(ValoresPagarConMercadoPagoDto valoresDeEntrada, ResultadoPagarMercadoPago resultadoPago)
        {
            var recorrido = servicio.ObtenerDatosRecorridoActivo(null, valoresDeEntrada.NumeroDeTarjeta);
            var pago = servicio.ObtenerPagoConMercadoPagoPorRecorridoId(recorrido.Id);
            if (pago == null)
            {
                var mensaje = "Pago no encontrado para el vehículo con tarjeta: " + valoresDeEntrada.NumeroDeTarjeta;
                logger.Warn(mensaje);
                resultadoPago.Errores.Add("", mensaje);
                return null;
            }
            return pago;
        }

    }
}
