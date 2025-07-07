using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Firmware;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.PagoDeReciboMunicipal)]
    public class PagoDeReciboMunicipalController : BaseController
    {
        private readonly ILogger logger;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioActividadFactory<IEjecutarService> factory;
        private readonly IListaDeWorkflows workflows;
        private readonly IRecorridoWorkflow recorridoWorkflow;

        public PagoDeReciboMunicipalController(IServicioRepositorio servicio, ILogger logger, IServicioComandos servicioComandos, IServicioActividadFactory<IEjecutarService> factory, IListaDeWorkflows workflows, IRecorridoWorkflow recorrido)
            : base(servicio)
        {
            this.logger = logger;
            this.factory = factory;
            this.servicioComandos = servicioComandos;
            this.workflows = workflows;
            this.recorridoWorkflow = recorrido ?? throw new ArgumentNullException(nameof(recorrido));
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
        public ActionResult PagarConEfectivo(ValoresPagoTasaMunicipalEfectivoDto valoresDeEntrada)
        {
            var resultadoPago = new Resultado();

            try
            {
                AvanzarProximaEtapa(valoresDeEntrada.NumeroDeTarjeta, valoresDeEntrada.PuestoDeTrabajoId);
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

            var reciboMunicipal = servicio.ObtenerRecorridoImpresionReciboMunicipalPorTarjeta(numeroDeTarjeta);

            return Json(reciboMunicipal, JsonRequestBehavior.AllowGet);
        }


        protected void AvanzarProximaEtapa(string numeroDeTarjeta, int puestoDeTrabajoId)
        {
            try
            {
                logger.Debug("EjecutarWorkflow. Tarjeta: {0} Puesto: {1}",
                numeroDeTarjeta, puestoDeTrabajoId);

                var proximaAccion = recorridoWorkflow.ObtenerWorkflowProximaAccion(numeroDeTarjeta, puestoDeTrabajoId);

                var workflowId = proximaAccion.WorkflowDefinicionId;
                var instanceId = proximaAccion.InstanceId;
                var proximaActividad = proximaAccion.ProximaActividad;
                puestoDeTrabajoId = proximaAccion.PuestoDeTrabajoId;

                var serviciowf = factory.CrearServicio(workflowId);
                var resultadoActividad = serviciowf.Ejecutar(instanceId, new ControlRecorridoDto
                {
                   WorkflowInstanceId = instanceId,
                   NombreUsuario = String.Empty,
                   Actividad = Textos.ResourceManager.GetString("Act" + proximaActividad) ?? proximaActividad,
                   ActividadXaml = proximaActividad,
                   Decision = true,
                   PuestoDeTrabajoId = puestoDeTrabajoId
                });

                if (resultadoActividad != null && resultadoActividad.HayErrores)
                {
                    string mensajeError = $"Error al ejecutar la actividad {proximaActividad} " +
                                          $"en el workflow: {workflowId}. " +
                                          $"Errores: {string.Join(", ", resultadoActividad.Errores.Select(e => $"{e.Key}: {e.Value}"))}";
                        
                    throw new InvalidOperationException(mensajeError);
                }
                
                else
                {
                    logger.Warn("No se puede ejecutar el WF relacionado con la tarjeta {0}", numeroDeTarjeta);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error al avanzar a la proxima etapa del workflow: {e.Message}", e);
            }
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
