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
using Newtonsoft.Json;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.PagoDeReciboMunicipal)]
    public class PagoDeReciboMunicipalController : BaseController
    {
        private readonly ILogger logger;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioActividadFactory<IEjecutarService> factory;
        private readonly IListaDeWorkflows workflows;

        public PagoDeReciboMunicipalController(IServicioRepositorio servicio, ILogger logger, IServicioComandos servicioComandos, IServicioActividadFactory<IEjecutarService> factory, IListaDeWorkflows workflows)
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
            var consultaPuestoDeTrabjo = servicio.ListarPuestosDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId);
            var puestoDeTrabajo = consultaPuestoDeTrabjo.Where(x => x.PidePatente).FirstOrDefault();
            var puestoDeTrabajoEf = consultaPuestoDeTrabjo.Where(x => !x.PidePatente).FirstOrDefault();

            ViewBag.PuestoDeTrabajoId = puestoDeTrabajo != null ? puestoDeTrabajo.Id : 0;
            ViewBag.Garita = puestoDeTrabajo != null ? puestoDeTrabajo.NombreGarita : "";
            ViewBag.CentroId = datosUsuario.CentroId;
            ViewBag.PuestoDeTrabajoIdEf = puestoDeTrabajoEf != null ? puestoDeTrabajoEf.Id : 0;

            return View();
        }

        [HttpPost]
        public ActionResult PagarConMercadoPago(ValoresPagarConMercadoPagoDto valoresDeEntrada)
        {
            var resultadoPago = new ResultadoPagarMercadoPago();
            try
            {
                logger.Debug($"PagarConMercadoPago {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId}");
                var resultadoValidacion = ValidarTarjeta(valoresDeEntrada, resultadoPago);
                if (!resultadoPago.HayErrores)
                {
                    logger.Debug($"PagarConMercadoPago - procesando pago - {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId}");
                    resultadoPago = (ResultadoPagarMercadoPago)servicioComandos.Ejecutar(new PagarMercadoPago { dto = valoresDeEntrada });

                    if (!resultadoPago.HayErrores)
                    {
                        logger.Debug($"PagarConMercadoPago - avanzando camion - {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId}");
                        AvanzarProximaEtapa(resultadoValidacion, resultadoPago);
                    }
                    else
                    {
                        logger.Debug($"PagarConMercadoPago - error al procesar el pago - {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId}");
                    }
                }
                else if (resultadoPago.Errores.ContainsKey("2"))
                {
                    logger.Debug($"PagarConMercadoPago - camion ya realizo el pago en el dia - avanzando camion - {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId} ");

                    AvanzarProximaEtapa(resultadoValidacion, resultadoPago);
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

            try
            {
                var resultadoValidacion = ValidarTarjeta(valoresDeEntrada, resultadoPago);
                if (!resultadoPago.HayErrores)
                {
                    AvanzarProximaEtapa(resultadoValidacion, resultadoPago);
                }
                else if (resultadoPago.Errores.ContainsKey("2"))
                {
                    logger.Debug($"PagarConMercadoPago - camion ya realizo el pago en el dia - avanzando camion - {valoresDeEntrada.NumeroDeTarjeta} {valoresDeEntrada.PuestoDeTrabajoId} {valoresDeEntrada.RecorridoId} ");
                    AvanzarProximaEtapa(resultadoValidacion, resultadoPago);
                }
            }
            catch (Exception e)
            {
                var mensajeError = (e.InnerException.Message == "cobroRealizado") ? e.Message : "Hubo un error al Tratar de imprimir el Ticket.";
                resultadoPago.Errores.Add("", mensajeError);
            }
            return Json(resultadoPago);
        }
        
        public ActionResult ObtenerDatos(string numeroDeTarjeta, int puestodetrabajoId)
        {
            var resultadoPago = new ResultadoPagarMercadoPago();

            var resultadoValidacion = ValidarTarjeta(new ValoresPagarConMercadoPagoDto { NumeroDeTarjeta = numeroDeTarjeta, PuestoDeTrabajoId = puestodetrabajoId }, resultadoPago);
            
            if (resultadoPago.HayErrores)
            {
                return Json(new { HayErrores = true, Error = resultadoPago.Errores.First() }, JsonRequestBehavior.AllowGet);
            }
            var reciboMunicipal = servicio.ObtenerRecorridoImpresionReciboMunicipalPorTarjeta(numeroDeTarjeta);

            return Json(reciboMunicipal, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public ActionResult AutorizarMercadoPago(string code)
        {
            if (String.IsNullOrEmpty(code))
            {
                return Redirect("https://auth.mercadopago.com.ar/authorization?client_id=205309017804878&response_type=code&platform_id=mp&redirect_uri=https%3A%2F%2Flocalhost/Scato.Web/PagoDeReciboMunicipal/AutorizarMercadoPago");
            }
            var resultado = (ResultadoCrear)servicioComandos.Ejecutar(new AutorizarMercadoPago { CodigoDeAutorizacion = code });
            return RedirectToAction("Index");
        }

        private void AvanzarProximaEtapa(ValidarProximaAccionDto resultadoValidacion, ResultadoPagarMercadoPago resultadoPago)
        {
            var serviciowf = factory.CrearServicio(resultadoValidacion.WorkflowDefinicionId);
            var resultadoActividad = serviciowf.Ejecutar(resultadoValidacion.InstanceId, new ControlRecorridoDto
            {
                WorkflowInstanceId = resultadoValidacion.InstanceId,
                NombreUsuario = String.Empty,
                Actividad = Textos.ResourceManager.GetString("Act" + resultadoValidacion.ProximaActividad) ?? resultadoValidacion.ProximaActividad,
                ActividadXaml = resultadoValidacion.ProximaActividad,
                Decision = true,
                PuestoDeTrabajoId = resultadoValidacion.PuestoDeTrabajoId
            });
            if (resultadoActividad != null && resultadoActividad.HayErrores)
            {
                var mensaje = $"La ejecución de la actividad {resultadoValidacion.ProximaActividad} terminó con errores: {resultadoActividad.Errores.First().Value}";
                logger.Error(mensaje);
                resultadoPago.Errores.Add("", mensaje);
            }
        }

        private ValidarProximaAccionDto ValidarTarjeta(ValoresPagarConMercadoPagoDto valoresDeEntrada, ResultadoPagarMercadoPago resultadoPago)
        {
            var materialPagoRealizado = new List<string>
            {
                Constantes.MaterialPagoRealizado.BiodiselAgranel, 
                Constantes.MaterialPagoRealizado.AceiteGirasolCrudoSAP
            };

            var recorrido = servicio.ObtenerDatosRecorridoActivo(null, new List<string> { valoresDeEntrada.NumeroDeTarjeta });
            if (recorrido == null)
            {
                var mensaje = "Error al obtener el vehículo por tarjeta: " + valoresDeEntrada.NumeroDeTarjeta;
                logger.Warn(mensaje);
                resultadoPago.Errores.Add("", mensaje);
                return null;
            }
            var proximaActividad = workflows.ObtenerWorkflowProximaAccion(recorrido.InstanciaWorkflow);
            if (!String.IsNullOrEmpty(proximaActividad.Mensaje))
            {
                var mensaje = "Error al obtener la proxima accion: " + proximaActividad.Mensaje;
                logger.Warn(mensaje);
                resultadoPago.Errores.Add("", mensaje);
                return null;
            }
            logger.Debug($"PagarReciboMuniciaplValidarTarjeta: {JsonConvert.SerializeObject(recorrido)}");

            //validar el vehiculo ya realizo el pago en el dia
            var validarPagoRealizado = servicio.ExistePagoRealizado(recorrido.Patente);
            var material = servicio.ObtenerMaterialPorWorkflow(recorrido.WorkflowId);

            //validar que hizo el pago en el dia y es Biodisel (Revisar) 
            if (validarPagoRealizado == false && materialPagoRealizado.Contains(material?.MaterialCodigoSap))
            {
                var mensaje = $"El vehículo {recorrido.Patente} ya realizó el pago de tasa municipal";
                logger.Warn(mensaje);
                resultadoPago.Errores.Add("2", mensaje);
                //return null;
            }

            if (proximaActividad.ProximaAccion != "EnPlayaExterna")
            {
                //var mensaje = $"El vehículo {recorrido.Patente} no se encuentra en la etapa \"En Playa externa\".";
                var mensaje = $"El vehículo {recorrido.Patente} se encuentra en la etapa {proximaActividad.ProximaAccion}";
                logger.Warn(mensaje);
                resultadoPago.Errores.Add("", mensaje);
                return null;
            }
            var puestos = new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Lectura = valoresDeEntrada.NumeroDeTarjeta, Id = valoresDeEntrada.PuestoDeTrabajoId } };
            var resultado = servicio.ValidarProximaActividadPorPuestoSinPatente(recorrido, proximaActividad.ProximaAccion, puestos);
            if (!resultado.Valida)
            {
                var mensaje = $"No se puede ejecutar el WF relacionado con la tarjeta {valoresDeEntrada.NumeroDeTarjeta}. Mensaje: {resultado.MensajeError}";
                logger.Warn(mensaje);
                resultadoPago.Errores.Add("", mensaje);
                return resultado;
            }
            return resultado;
        }

        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario)
        {
            ViewBag.Items = workflows.ListarWorkFlows(new Paginacion("FechaUltimaModificacion", DirOrden.Desc, 1, 5), 
                new FiltroListaDeWorkflowsDto
                {
                    ProximaAccion = "EnTransito"
                });
            return View();
        }
    }
}
