using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadCaladoEnPlanta)]
    public class CaladoEnPlantaController : BaseController
    {
        private readonly IServicioActividadFactory<ICaladoEnPlantaService> factory;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioComandos servicioComandos;
        private readonly IConfiguracionProvider configuracion;
        private ILogger log;

        public CaladoEnPlantaController(ILogger log, IServicioActividadFactory<ICaladoEnPlantaService> factory, IServicioRepositorio servicio, IServicioOrquestador orquestador, IServicioComandos servicioComandos, IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.factory = factory;
            this.orquestador = orquestador;
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            log.Info("{0} - Index", id);
            var recorrido = SetearVista(id);

            var caracteristicas = servicio.ListarCaracteristicasDeCalidadPorMaterialWorkflow(recorrido.Material.Id, recorrido.Centro.Id, recorrido.Workflow.Id).OrderByDescending(x => x.EsHumedad).ThenByDescending(x => x.PrioridadEnCalado).ThenBy(x => x.Descripcion);

            if (caracteristicas.Any(x => x.Dispositivo.Equals(TipoDispositivo.Humedimetro)))
            {
                var humedimetro = servicio.ObtenerHumedimetroPorNombrePc(datosUsuario.CentroId, datosUsuario.NombrePc);
                ViewBag.Humedimetro = humedimetro;

                if (humedimetro == null)
                {
                    TempData["Alerta"] = String.Format(Textos.Humedimetro_PuestoDeTrabajoInexistente);
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
            }

            if (caracteristicas.Any(x => x.Dispositivo.Equals(TipoDispositivo.NIRS)))
            {
                var nirs = servicio.ObtenerNirsPorNombrePc(datosUsuario.CentroId, datosUsuario.NombrePc);
                ViewBag.Nirs = nirs;

                if (nirs == null)
                {
                    TempData["Alerta"] = String.Format(Textos.Nirs_PuestoDeTrabajoInexistente);
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
            }

            var valores = servicio.ListarAnalisisYCaladoPorCaracteristica(id);

            log.Info("{0} - Fin - Index", id);
            
            return View(new CaladoEnPlantaDto
            {
                CaladosPorCaracteristica = caracteristicas.Select(
                        x => new CaladoEnPlantaPorCaracteristicaDto
                        {
                            Unidad = x.UnidadDeMedida,
                            RangoMin = x.CaladoMinimo,
                            RangoMax = x.CaladoMaximo,
                            CaladoObligatorio = x.CargaEnCalado,
                            Caracteristica = x.Descripcion,
                            CaracteristicaId = x.Id,
                            EsHumedad = x.EsHumedad,
                            ValorCalado = valores.Where(y => y.CaracteristicaId == x.Id).Select(z => z.ValorAnalisis ?? z.ValorCalado ?? null).FirstOrDefault(),
                            Dispositivo = x.Dispositivo,
                            CaracteristicaNombreNirs = x.NombreNirs,
                            Modalidad = x.Dispositivo == TipoDispositivo.NIRS ? ((NirsDto)ViewBag.Nirs).Modalidad : (x.Dispositivo == TipoDispositivo.Humedimetro ? ((HumedimetroDto)ViewBag.Humedimetro).Modalidad : Modalidad.Manual)
                        }).ToList(),
                WorkflowDefinicionId = recorrido.WorkflowDefinicionId,
                WorkflowInstanceId = recorrido.InstanciaWorkflow,
                FechaCreacion = DateTime.Now,
                NirsCodigoProducto = servicio.ObtenerMaterialNirsCodigoProducto(recorrido.Material.Id) ?? "",
                MaterialId = recorrido.Material.Id,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                RecorridoId = recorrido.Id,
                CentroId = recorrido.Centro.Id,
                NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso
            });
        }

        private RecorridoDto SetearVista(Guid id)
        {
            log.Info("{0} - SetearVista", id);
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            var vehiculo = recorrido.Vehiculo;
            var info = servicio.ObtenerInformacionCartaPorte(recorrido.Id);

            ViewBag.Patente = recorrido.Patente;
            ViewBag.PatenteAcoplado = vehiculo != null ? vehiculo.PatenteAcoplado : string.Empty;
            ViewBag.FechaIngreso = string.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", recorrido.FechaInicio);
            ViewBag.TipoComercial = recorrido.TipoComercial.Descripcion;
            ViewBag.TitularCP = info.TitularCartaPorte;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.TarjetaDeAcceso = recorrido.TarjetaDeAcceso;
            ViewBag.AgenteCompras = !string.IsNullOrEmpty(info.AgenteCompras) ? info.AgenteCompras : Textos.No;
            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.TieneEntregador = !string.IsNullOrEmpty(info.Entregador) ? info.Entregador : Textos.No;
            ViewBag.RtteComercial = !string.IsNullOrEmpty(info.RtteComercial) ? info.RtteComercial : Textos.No;
            ViewBag.CTG = !string.IsNullOrEmpty(info.CTG) ? info.CTG : Textos.No;
            ViewBag.MotivoManual = servicio.ListarMotivosHumedad().ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            ViewBag.RangosDeRedondeo = servicio.ListarRangosDeRedondeoPorMaterial(recorrido.Material.Id).ToJson();

            ViewBag.TipoVehiculo = recorrido.TipoVehiculo;
            ViewBag.EsEgreso = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso;
            ViewBag.Material = recorrido.Material.Descripcion;
            ViewBag.Rechazado = recorrido.Rechazado;
            ViewBag.TrigoEspecial = info.TrigoEspecial;
            ViewBag.EsSojaSustentable = recorrido.Establecimiento != null;
            ViewBag.RecorridoId = recorrido.Id;

            log.Info("{0} - Fin - SetearVista", id);
            return recorrido;
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult CaladoEnPlanta(CaladoEnPlantaDto caladoEnPlantapantalla, DatosUsuario datosUsuario)
        {
            log.Info("{0} - CaladoEnPlanta", caladoEnPlantapantalla.WorkflowInstanceId);
            var caracteristicas = new CaladoEnPlantaPorCaracteristicaDto[0];
            caladoEnPlantapantalla.FechaCreacion = DateTime.Now;
            caladoEnPlantapantalla.Usuario = datosUsuario.NombreUsuario;
            if (caladoEnPlantapantalla.CaladosPorCaracteristica != null)
            {
                caracteristicas = caladoEnPlantapantalla.CaladosPorCaracteristica.Where(x => x.ValorCaladoEnPlanta.HasValue).ToArray();
            }

            log.Debug("{0} - CaladoEnPlanta Creo ControlRecorridoDto", caladoEnPlantapantalla.WorkflowInstanceId);
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = caladoEnPlantapantalla.WorkflowInstanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActCaladoEnPlanta,
                ActividadXaml = "CaladoEnPlanta",
                Decision = false,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                Comentario = caladoEnPlantapantalla.Comentario
            };
            log.Debug("{0} - CaladoEnPlanta llamo al servicio", caladoEnPlantapantalla.WorkflowInstanceId);
            var serivce = factory.CrearServicio(caladoEnPlantapantalla.WorkflowDefinicionId);
            var resultado = serivce.CaladoEnPlanta(caladoEnPlantapantalla.WorkflowInstanceId, caladoEnPlantapantalla, controlRecorrido);
            log.Debug("{0} - CaladoEnPlanta - llamada al servicio completa", caladoEnPlantapantalla.WorkflowInstanceId);
            if (!resultado.HayErrores)
            {
                log.Debug("{0} - CaladoEnPlanta - no hay errores, guardo la muestra de humedad", caladoEnPlantapantalla.WorkflowInstanceId);
                GuardarMuestraDeHumedad(caladoEnPlantapantalla, datosUsuario);
                GuardarMuestraDeNirs(caladoEnPlantapantalla, datosUsuario);
                log.Debug("{0} - Fin - CaladoEnPlanta", caladoEnPlantapantalla.WorkflowInstanceId);
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            TempData["Alerta"] = Textos.Calado_ErrorEnLaCarga;
            TempData["TipoAlerta"] = TipoAlerta.Error;
            log.Error("{0} - Fin - CaladoEnPlanta", caladoEnPlantapantalla.WorkflowInstanceId);
            return RedirectToAction("Index", new { id = caladoEnPlantapantalla.WorkflowInstanceId });
        }

        [DatosUsuario]
        public ActionResult Rechazar(string codigoWf, int workflowDefinicionId, Guid instanceId, DatosUsuario datosUsuario)
        {
            log.Info("{0} - CaladoEnPlanta Rechazar", instanceId);
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
            ViewBag.Workflow = codigoWf;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActCaladoEnPlanta + "/" + Textos.Rechazar,
                ActividadXaml = "CaladoEnPlanta",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                Decision = true
            };

            return View("_TransportistaRechazado", controlRecorrido);
        }

        [DatosUsuario]
        public ActionResult TransportistaRechazado(string workflow, int workflowDefinicionId, CaladoEnPlantaDto caladoEnPlantapantalla, ControlRecorridoDto controlRecorrido)
        {
            log.Info("{0} - CaladoEnPlanta rechazado", workflowDefinicionId);
            controlRecorrido.Decision = true;
            var serivce = factory.CrearServicio(workflowDefinicionId);
            var resultado = serivce.CaladoEnPlanta(controlRecorrido.WorkflowInstanceId, caladoEnPlantapantalla, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }

        private void GuardarMuestraDeHumedad(CaladoEnPlantaDto caladoEnPlantapantalla, DatosUsuario datosUsuario)
        {
            log.Info("{0} - CaladoEnPlanta GuardarMuestraDeHumedad", caladoEnPlantapantalla.WorkflowInstanceId);
            if (caladoEnPlantapantalla.CaladosPorCaracteristica != null)
            {
                var humedad = caladoEnPlantapantalla.CaladosPorCaracteristica.FirstOrDefault(x => x.EsHumedad);
                if (humedad != null)
                {
                    var modalidad = humedad.Dispositivo == TipoDispositivo.Humedimetro ? caladoEnPlantapantalla.HumedimetroModalidad :
                                        (humedad.Dispositivo == TipoDispositivo.NIRS ? humedad.Modalidad : Modalidad.Manual);
                    servicioComandos.Ejecutar(new CrearMuestraDeHumedad
                    {
                        Dto = new MuestraDeHumedadDto
                        {
                            CentroId = caladoEnPlantapantalla.CentroId,
                            HumedimetroId = caladoEnPlantapantalla.HumedimetroId,
                            CicloDeCalado = 1,
                            Fecha = DateTime.Now,
                            NroDeToma = humedad.NroDeToma,
                            Usuario = datosUsuario.NombreUsuario,
                            ValorFinal = humedad.ValorCaladoEnPlanta ?? 0,
                            ValorLeido = modalidad == Modalidad.Manual ? (humedad.ValorCaladoEnPlanta ?? 0) : (humedad.ValorAutomatico ?? 0),
                            Modalidad = modalidad == Modalidad.Manual ? Textos.Manual : Textos.Automatico,
                            NumeroDocumentoIngreso = caladoEnPlantapantalla.NumeroDocumentoIngreso,
                            WorkflowInstanceId = caladoEnPlantapantalla.WorkflowInstanceId,
                            NumeroOrden = string.Empty,
                            MotivoHumedadManualId = modalidad == Modalidad.Automática && humedad.ValorCaladoEnPlanta != humedad.ValorAutomatico ? caladoEnPlantapantalla.MotivoHumedadManualId : null,
                            Dispositivo = humedad.Dispositivo == TipoDispositivo.Humedimetro ? "GAC" : (humedad.Dispositivo == TipoDispositivo.NIRS ? "NIRS" : "")
                        }
                    });
                    log.Info("{0} - CaladoEnPlanta - Muestra guardada", caladoEnPlantapantalla.WorkflowInstanceId);
                }
            }
            log.Info("{0} - CaladoEnPlanta Fin - GuardarMuestraDeHumedad", caladoEnPlantapantalla.WorkflowInstanceId);
        }

        private void GuardarMuestraDeNirs(CaladoEnPlantaDto caladoEnPlantapantalla, DatosUsuario datosUsuario)
        {
            log.Info("{0} - CaladoEnPlanta GuardarMuestraDeNirs", caladoEnPlantapantalla.WorkflowInstanceId);
            if (caladoEnPlantapantalla.CaladosPorCaracteristica != null)
            {
                var valores = caladoEnPlantapantalla.CaladosPorCaracteristica.Where(x => x.Dispositivo.Equals(TipoDispositivo.NIRS));

                foreach (var valor in valores)
                {
                    servicioComandos.Ejecutar(new CrearMuestraDeNirs
                    {
                        Dto = new MuestraDeNirsDto
                        {
                            CentroId = caladoEnPlantapantalla.CentroId,
                            NirsId = caladoEnPlantapantalla.NirsId,
                            CicloDeCalado = 1,
                            Fecha = DateTime.Now,
                            NroDeToma = valor.NroDeToma,
                            Usuario = datosUsuario.NombreUsuario,
                            ValorFinal = valor.ValorCaladoEnPlanta ?? 0,
                            ValorLeido = valor.Modalidad == Modalidad.Manual ? (valor.ValorCaladoEnPlanta ?? 0) : (valor.ValorAutomatico ?? 0),
                            Modalidad = valor.Modalidad == Modalidad.Manual ? Textos.Manual : Textos.Automatico,
                            NumeroDocumentoIngreso = caladoEnPlantapantalla.NumeroDocumentoIngreso,
                            WorkflowInstanceId = caladoEnPlantapantalla.WorkflowInstanceId,
                            NumeroOrden = string.Empty,
                        }
                    });
                    log.Info("{0} - Nirs - Muestra guardada", caladoEnPlantapantalla.WorkflowInstanceId);
                }
            }
            log.Info("{0} - Nirs Fin - GuardarMuestraDeNirs", caladoEnPlantapantalla.WorkflowInstanceId);
        }


    }
}
