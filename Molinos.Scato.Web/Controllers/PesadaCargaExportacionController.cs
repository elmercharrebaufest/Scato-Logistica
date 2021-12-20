using System;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadPesadaCargaExportacion)]
    public class PesadaCargaExportacionController : PesadaController
    {
        private readonly IServicioActividadFactory<IEjecutarService> factoryEjecutar;

        public PesadaCargaExportacionController(ILogger log, IServicioRepositorio servicio,
                                IServicioActividadFactory<IPesadaService> factory, IServicioComandos comando,
                                IServicioOrquestador orquestador, IListaDeWorkflows workflows,
                                IServicioActividadFactory<IEjecutarService> factoryEjecutar)
            : base(log, servicio, factory, comando, orquestador, workflows)
        {
            ActividadXaml = "PesadaCargaExportacionInicio";
            this.factoryEjecutar = factoryEjecutar;
        }

        [DatosUsuario]
        public override ActionResult Index(Guid id, DatosUsuario datosUsuario, bool automatizado = false)
        {
            log.Debug("Puesto de Trabajo Index {0}", datosUsuario.PuestoDeTrabajoId);

            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            BalanzaDto balanza;
            if (datosUsuario.PuestoDeTrabajoId > 0)
            {
                balanza = servicio.ObtenerBalanzaPorPuestoDeTrabajo(datosUsuario.PuestoDeTrabajoId, recorrido.TipoVehiculo);
            }
            else
            {
                balanza = servicio.ListarBalanzasPorNombrePc(datosUsuario.CentroId, datosUsuario.NombrePc, recorrido.TipoVehiculo).FirstOrDefault();
            }


            return View(new PesadaCargaExportacion
            {
                WorkflowInstanceId = recorrido.InstanciaWorkflow,
                Patente = recorrido.Patente,
                ActividadXaml = ActividadXaml,
                TipoVehiculo = recorrido.TipoVehiculo,
                Rechazado = recorrido.Rechazado,
                DocumentoIngreso = recorrido.TipoDocumentoIngreso,
                NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso,
                Material = recorrido.Material != null ? recorrido.Material.Descripcion : "",
                TipoComercial = recorrido.TipoComercial.Descripcion,
                Workflow = recorrido.Workflow.Codigo,
                WorkflowDefinicionId = recorrido.WorkflowDefinicionId,
                TieneEntregador = recorrido.Vehiculo != null && servicio.CartaPorteTieneEntregador(recorrido.Vehiculo.CartaPorteId)
                                              ? Textos.Si
                                              : Textos.No,
                PatenteOriginal = recorrido.Patente,
                BalanzaId = balanza != null ? balanza.Id : 0,
                Balanza = balanza != null ? balanza.Nombre : "",
                Modalidad = balanza != null ? (int)balanza.Modalidad : 0,
                EstaEnCero = balanza != null ? balanza.EstaEnCero : true,
                Color = balanza != null ? balanza.Color : ""
            });
        }

        [DatosUsuario]
        public ActionResult PesadaTara(Guid id, string balanza, string color, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPesadaExportacion(id);
            ViewBag.Etapa = ObtenerEtapa(recorrido);
            return View("_PesadaTara", new Pesada
            {
                Peso = recorrido.PesoTara,
                Balanza = balanza,
                Color = color
            });
        }

        [DatosUsuario]
        public ActionResult PesadaBruto(Guid id, string balanza, string color, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPesadaExportacion(id);
            ViewBag.Etapa = ObtenerEtapa(recorrido);
            return View("_PesadaBruto", new Pesada
            {
                Peso = recorrido.PesoBruto,
                Balanza = balanza,
                Color = color
            });
        }

        [DatosUsuario]
        public ActionResult Carga(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPesadaExportacion(id);
            ViewBag.WorkflowInstanceId = recorrido.InstanciaWorkflow;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            ViewBag.Etapa = ObtenerEtapa(recorrido);
            ViewBag.Peso = servicio.ObtenerIngresoDeDatosDeExportacionPorRecorrido(recorrido.Id).PesoNeto ?? 0;
            return View("_Carga");
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Carga(Guid workflowInstanceId, bool cargaParcial, string workflow, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = workflowInstanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActConfirmacionDeCargaDescarga,
                ActividadXaml = "PesadaCargaExportacion/Carga",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                Decision = cargaParcial,
                //Mensaje = "Cancelado"
            };

            var serviciowf = factoryEjecutar.CrearServicio(workflowDefinicionId);

            var resultado = serviciowf.Ejecutar(controlRecorrido.WorkflowInstanceId, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return Json(new { responseText = "OK", CargaParcial = cargaParcial });
            }
            return new ContentResult { Content = resultado.Errores.Values.FirstOrDefault() ?? Textos.Error_ActualizarGenerico };
        }
        
        [DatosUsuario]
        public ActionResult Cancelar(Guid workflowInstanceId, int workflowDefinicionId, DatosUsuario datosUsuario, string workflow = "")
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = workflowInstanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActConfirmacionDeCargaDescarga,
                ActividadXaml = "PesadaCargaExportacion/Carga",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                Mensaje = "Cancelado"
            };

            comando.Ejecutar(new ModificarRecorridoPeso {BalanzaId = null, InstanceId = workflowInstanceId, Peso = null, TipoPesada = Dominio.Enums.TipoPesada.Tara, Usuario = controlRecorrido.NombreUsuario});

             var serviciowf = factoryEjecutar.CrearServicio(workflowDefinicionId);

            var resultado = serviciowf.Ejecutar(controlRecorrido.WorkflowInstanceId, controlRecorrido);
            if (!resultado.HayErrores)
            {
                //return RedirectToAction("Index", "ListaDeCamiones");
                return Json(new { responseText = "OK", Mensaje = "Cancelado" }, JsonRequestBehavior.AllowGet);
            }
            return new ContentResult { Content = resultado.Errores.Values.FirstOrDefault() ?? Textos.Error_ActualizarGenerico };
        }

        private PermisosScato ObtenerEtapa(RecorridoDto recorrido)
        {
            var ultimoLog = servicio.ObtenerUltimoLog(recorrido.InstanciaWorkflow);
            return ultimoLog.Actividad != "Pesada Bruto Exportacion" && ultimoLog.Actividad != "Pesada Tara Exportacion" && ultimoLog.Actividad != "Confirmacion de Carga/Descarga" ? PermisosScato.AbmListaDeTareasAutomatizada :
                !recorrido.PesoTara.HasValue ? PermisosScato.ActividadPesadaTara :
                ultimoLog.Actividad == "Pesada Bruto Exportacion" ? PermisosScato.ActividadPesadaBruto :
                PermisosScato.ActividadConfirmacionDeCargaDescarga;
        }
    }
}
