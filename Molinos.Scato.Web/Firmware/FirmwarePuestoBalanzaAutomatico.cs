using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.ServicioHub;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace Molinos.Scato.Web.Firmware
{
    public class FirmwarePuestoBalanzaAutomatico : FirmwareBase
    {
        private readonly IServicioActividadFactory<IPesadaService> pesadaFactory;
        private readonly IServicioEstadoPuesto estadoPuesto;

        public FirmwarePuestoBalanzaAutomatico(ILogger log, 
            IServicioRepositorio servicioRepositorio, 
            IListaDeWorkflows workflows,
            IServicioComandos comandos,
            IServicioOrquestador servicioOrquestador,
            IServicioActividadFactory<IPesadaService> pesadaFactory,
            IServicioActividadFactory<IEjecutarService> factory,
            IServicioEstadoPuesto estadoPuesto,
            HubClientFactory hubClientFactory) : base(
                log, servicioRepositorio, workflows, comandos, servicioOrquestador, factory, hubClientFactory)
        {
            this.pesadaFactory = pesadaFactory;
            this.estadoPuesto = estadoPuesto;
        }

        public override void ProcesarEvento(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            if (lecturaPuestoDeTrabajo.TarjetaValida)
            {
                var recorrido = ObtenerRecorrido(lecturaPuestoDeTrabajo);
                var proximaActividad = new ProximaAccionDto();
                if (recorrido != null)
                {
                    proximaActividad = workflows.ObtenerWorkflowProximaAccion(recorrido.InstanciaWorkflow);
                }
                if (!string.IsNullOrEmpty(proximaActividad.Mensaje))
                {
                    log.Warn("Error al obtener la proxima accion: " + recorrido?.ProximaAccionMensaje);
                    lecturaPuestoDeTrabajo.MensajeError = recorrido.ProximaAccionMensaje;
                    NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, recorrido, "");
                    return;
                }

                log.Debug($"Validando puesto sin patente. Tarjeta: {lecturaPuestoDeTrabajo.NumeroDeTarjeta} Puesto: {lecturaPuestoDeTrabajo.PuestoDeTrabajoId}");
                // paso por las etapas sin pesada
                if (proximaActividad.ProximaAccion.ToLower().Contains("enesperaaduana")
                    || proximaActividad.ProximaAccion.ToLower().Contains("pasoporbalanza"))
                {
                    EjecutarPuestoDesatendido(lecturaPuestoDeTrabajo, recorrido);
                }
                //todas las etapas que no sean pesada muestra mensaje
                else if (!proximaActividad.ProximaAccion.ToLower().Contains("pesada"))
                //if (recorrido.ProximaAccion.ToLower().Contains("autorizartiempoentransito"))
                {
                    lecturaPuestoDeTrabajo.MensajeError = "El camion se encuentra en etapa:" + proximaActividad.ProximaAccion;
                    NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, recorrido, "");
                }
                else if (proximaActividad.ProximaAccion.ToLower().Contains("pesadacargaexportacion"))
                //if (recorrido.ProximaAccion.ToLower().Contains("autorizartiempoentransito"))
                {
                    NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, recorrido, "PesadaCargaExportacion");
                }
                else
                {
                    EjecutarPesadaAutomatica(lecturaPuestoDeTrabajo, recorrido);
                }                //else
                //{
                //    EjecutarPuestoDesatendido(lecturaPuestoDeTrabajo, recorrido);
                //}
            }
            else
            {
                lecturaPuestoDeTrabajo.MensajeError = $"Tarjeta no válida: {lecturaPuestoDeTrabajo.NumeroDeTarjeta}";
                NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, new DatosRecorridoDto(), "En Espera");
            }
        }

        private void EjecutarPesadaAutomatica(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido)
        {
            try
            {
                EjecutarDispositivos(lecturaPuestoDeTrabajo, recorrido);

                var resultado = ValidarProximaActividad(lecturaPuestoDeTrabajo, recorrido);
                lecturaPuestoDeTrabajo.MensajeError = null;

                if (resultado.Valida)
                {
                    if (!lecturaPuestoDeTrabajo.ReconocimientoExitoso)
                    {
                        lecturaPuestoDeTrabajo.MensajeError = "Patente no reconocida";
                        NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, recorrido, recorrido.ProximaAccion);
                        return;
                    }
                    while (true)
                    {
                        var valida = ConfigurationManager.AppSettings["ValidaCicloDePosicionamiento"];
                        if (estadoPuesto.ValidarEstadoPuesto(lecturaPuestoDeTrabajo.PuestoDeTrabajoId) || valida != "1")
                            break;
                        var tiempoDeCiclo = int.Parse(ConfigurationManager.AppSettings["TiempoDeCicloPosicionamiento"]);
                        Thread.Sleep(tiempoDeCiclo);
                    }
                    NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, recorrido, resultado.ProximaActividad);
                    var serviciowf = pesadaFactory.CrearServicio(resultado.WorkflowDefinicionId);
                    var resultadoActividad = serviciowf.Pesada(resultado.InstanceId,
                         0, 0, null, null, 0, null,
                         false, DateTime.Now, new ControlRecorridoDto
                         {
                             WorkflowInstanceId = resultado.InstanceId,
                             NombreUsuario = String.Empty,
                             Actividad = resultado.ProximaActividad,
                             ActividadXaml = resultado.ProximaActividad,
                             Decision = false,
                             PuestoDeTrabajoId = resultado.PuestoDeTrabajoId,
                             Automatizado = true,
                             CartaDePorte = recorrido.CartaDePorte,
                             Entregador = recorrido.Entregador,
                             Material = recorrido.Material,
                             Patente = recorrido.Patente,
                             PesoOrigenBruto = recorrido.PesoBrutoOrigen,
                             PesoOrigenTara = recorrido.PesoTaraOrigen,
                             PesoOrigenNeto = recorrido.PesoNetoOrigen,
                             PesoBruto = recorrido.PesoBruto,
                             PesoTara = recorrido.PesoTara,
                             TipoVehiculo = recorrido.TipoVehiculo,
                             Tarjeta = recorrido.TarjetaDeAcceso,
                             Calle = recorrido.Calle,
                             TipoDeWorkflow = recorrido.TipoDeWorkflow,
                             RecorridoId = recorrido.Id
                         });
                    if (resultadoActividad != null && resultadoActividad.HayErrores)
                    {
                        log.Error("La ejecución de la actividad {0} terminó con errores: {1}",
                                resultado.ProximaActividad, resultadoActividad.Errores.First().Value);
                    }
                }
                else
                {
                    log.Warn("No se puede ejecutar el WF relacionado con la tarjeta {0}. Mensaje: {1}",
                            lecturaPuestoDeTrabajo.NumeroDeTarjeta, resultado.MensajeError);
                    lecturaPuestoDeTrabajo.TarjetaValida = false;
                    lecturaPuestoDeTrabajo.MensajeError = resultado.MensajeError;
                    NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, recorrido, recorrido.ProximaAccion);
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Fallo la ejecucion del workflow relacionado con la tarjeta: {0}", lecturaPuestoDeTrabajo.NumeroDeTarjeta);
            }
        }

        private void NotificarBalanzadaPorSignalR(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido, string proximaActividad)
        {
            proximaActividad = proximaActividad ?? "";
            var notificacion = new NotificacionPesadaAutomaticaDto
            {
                Id = lecturaPuestoDeTrabajo.PuestoDeTrabajoId,
                Error = string.IsNullOrEmpty(lecturaPuestoDeTrabajo.MensajeError) ? null : lecturaPuestoDeTrabajo.MensajeError,
                CartaPorte = recorrido == null ? "" : recorrido.CartaDePorte,
                Diferencia = "0",
                Peso = "0",
                DifNeto = "0",
                DifPeso = "0",

                Entregador = recorrido == null ? "" : recorrido.Entregador ? "Si" : "No",
                Material = recorrido == null ? "material" : recorrido.Material,
                WorkflowInstanceId = recorrido == null ? new Guid() : recorrido.InstanciaWorkflow,
                TipoPeso = proximaActividad.Contains("Bruto") ? "Bruto:" : proximaActividad.Contains("Tara") ? "Tara:" : "Peso:",
                Patente = recorrido == null ? "" : recorrido.Patente,
                Actividad = proximaActividad,
                Tarjeta = lecturaPuestoDeTrabajo.NumeroDeTarjeta,
                NoRedirecciona = true,
                TipoVehiculo = recorrido == null ? "vehículo" : recorrido.TipoVehiculo.DisplayText(),
                FotoAlMarcarTarjeta = lecturaPuestoDeTrabajo.VideoCamaras != null && lecturaPuestoDeTrabajo.VideoCamaras.Any(),
                Calle = recorrido.Calle,
                TipoPesoOrigen = proximaActividad.Contains("Bruto") ? "Bruto Org:" : proximaActividad.Contains("Tara") ? "Tara Org:" : "Peso Org:",
                PesoBrutoOrigen = recorrido.PesoBrutoOrigen.ToString(),
                PesoNetoOrigen = recorrido.PesoNetoOrigen.ToString(),
                TipoComercial = recorrido.TipoComercial.ToString(),
                DocumentoIngreso = recorrido.TipoDocumento.ToString(),
                WorkflowDefinicionId = recorrido.WorkflowDefinicionId,
                PesoTara = recorrido.PesoTara
            };
            var notificacionDto = new NotificacionDto
            {
                Hora = DateTime.Now,
                Grupo = "Automaticas",
                Mensaje = notificacion.ToJson(),
                TipoAlerta = TipoAlerta.Automatica,
                Leido = string.IsNullOrEmpty(notificacion.Error),
                PuestoId = lecturaPuestoDeTrabajo.PuestoDeTrabajoId
            };
            NotificarPorSignalR(notificacionDto);
        }

    }
}


