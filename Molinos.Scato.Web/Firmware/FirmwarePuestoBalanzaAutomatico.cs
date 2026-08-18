using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.ServicioHub.Client;
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

        public FirmwarePuestoBalanzaAutomatico(
            ILogger log, 
            IServicioRepositorio servicioRepositorio, 
            IListaDeWorkflows workflows,
            IServicioComandos comandos,
            IServicioOrquestador servicioOrquestador,
            IServicioActividadFactory<IPesadaService> pesadaFactory,
            IServicioActividadFactory<IEjecutarService> factory,
            IServicioEstadoPuesto estadoPuesto,
            IRecorridoWorkflow recorridoWorkflow,
            HubClients hubClients) 
            : base(
                log, 
                servicioRepositorio, 
                workflows, 
                comandos, 
                servicioOrquestador, 
                factory, 
                hubClients, 
                recorridoWorkflow)
        {
            this.pesadaFactory = pesadaFactory;
            this.estadoPuesto = estadoPuesto;
        }

        public override string ProcesarEvento(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            if (!lecturaPuestoDeTrabajo.TarjetaValida)
            {
                lecturaPuestoDeTrabajo.MensajeError = $"Tarjeta no válida: {lecturaPuestoDeTrabajo.NumeroDeTarjeta}";
                NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, new DatosRecorridoDto(), "En Espera");
                return lecturaPuestoDeTrabajo.MensajeError;
            }

            var recorrido = ObtenerRecorrido(lecturaPuestoDeTrabajo);
            var proximaActividad = new ProximaAccionDto();
            if (recorrido != null)
                proximaActividad = workflows.ObtenerWorkflowProximaAccion(recorrido.InstanciaWorkflow);
                
            if (!string.IsNullOrEmpty(proximaActividad.Mensaje))
            {
                log.Warn("Error al obtener la proxima accion: " + recorrido?.ProximaAccionMensaje);
                lecturaPuestoDeTrabajo.MensajeError = recorrido.ProximaAccionMensaje;
                NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, recorrido, "");
                return lecturaPuestoDeTrabajo.MensajeError;
            }

            if (proximaActividad.ProximaAccion.ToLower().Contains("enesperaaduana") || proximaActividad.ProximaAccion.ToLower().Contains("pasoporbalanza"))
                return EjecutarPuestoDesatendido(lecturaPuestoDeTrabajo, recorrido);
           
            if (!proximaActividad.ProximaAccion.ToLower().Contains("pesada"))
            {
                lecturaPuestoDeTrabajo.MensajeError = "El camion se encuentra en etapa:" + proximaActividad.ProximaAccion;
                NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, recorrido, "");
                return lecturaPuestoDeTrabajo.MensajeError;
            }
            
            if (proximaActividad.ProximaAccion.ToLower().Contains("pesadacargaexportacion"))
            {
                NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, recorrido, "PesadaCargaExportacion");
                return !string.IsNullOrEmpty(lecturaPuestoDeTrabajo.MensajeError) ? lecturaPuestoDeTrabajo.MensajeError : Constantes.ResultadoProcesoIdentificacionVehicular.LecturaBalanza + "Pesada Carga Exportacion";
            }
            
            return EjecutarPesadaAutomatica(lecturaPuestoDeTrabajo, recorrido);
        }

        private string EjecutarPesadaAutomatica(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido)
        {
            try
            {
                EjecutarDispositivosConCardless(lecturaPuestoDeTrabajo, recorrido);
                
                var resultado = ValidarProximaActividad(lecturaPuestoDeTrabajo, recorrido);
                lecturaPuestoDeTrabajo.MensajeError = null;

                if (!resultado.Valida)
                {
                    log.Warn("No se puede ejecutar el WF relacionado con la tarjeta {0}. Mensaje: {1}", lecturaPuestoDeTrabajo.NumeroDeTarjeta, resultado.MensajeError);
                    lecturaPuestoDeTrabajo.TarjetaValida = false;
                    lecturaPuestoDeTrabajo.MensajeError = resultado.MensajeError;
                    NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, recorrido, recorrido.ProximaAccion);
                    return Constantes.ResultadoProcesoIdentificacionVehicular.LecturaBalanza + lecturaPuestoDeTrabajo.MensajeError;
                }

                if (!lecturaPuestoDeTrabajo.ReconocimientoExitoso)
                {
                    lecturaPuestoDeTrabajo.MensajeError = "Patente no reconocida";
                    NotificarBalanzadaPorSignalR(lecturaPuestoDeTrabajo, recorrido, recorrido.ProximaAccion);
                    return Constantes.ResultadoProcesoIdentificacionVehicular.LecturaBalanza + lecturaPuestoDeTrabajo.MensajeError;
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

                var huboErrorEnWorkflow = resultadoActividad != null && resultadoActividad.HayErrores;
                if (huboErrorEnWorkflow) log.Error("La ejecución de la actividad {0} terminó con errores: {1}", resultado.ProximaActividad, resultadoActividad.Errores.First().Value);
                return huboErrorEnWorkflow ? resultadoActividad.Errores.First().Value : Constantes.ResultadoProcesoIdentificacionVehicular.EjecucionExitosa;
            }
            catch (Exception e)
            {
                log.Error(e, "Fallo la ejecucion del workflow relacionado con la tarjeta: {0}", lecturaPuestoDeTrabajo.NumeroDeTarjeta);
                return e.Message;
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
                Tarjeta = recorrido == null ? "" : recorrido.TarjetaDeAcceso,
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

        private void EjecutarDispositivosConCardless(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido)
        {
            comandos.Ejecutar(new GuardarFotoIdentificacionVehicular
            {
                LogIdentificacionVehicularId = lecturaPuestoDeTrabajo.LogIdentificacionVehicularId.Value,
                CentroCodigoSap = recorrido.CentroCodigoSap,
                NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso,
                Patente = recorrido.Patente,
                ProximaActividad = recorrido.ProximaAccion,
                TipoVehiculo = recorrido.TipoVehiculo,
            });

            foreach (var dispositivo in lecturaPuestoDeTrabajo.Entrada)
            {
                var resultado = servicioOrquestador.Ejecutar(new EjecutarAperturaBarrera
                {
                    CodigoDispositivo = dispositivo
                });
            }

            foreach (var dispositivo in lecturaPuestoDeTrabajo.Salida)
            {
                var resultado = servicioOrquestador.Ejecutar(new EjecutarCierreBarrera
                {
                    CodigoDispositivo = dispositivo
                });
            }
        }
    }
}


