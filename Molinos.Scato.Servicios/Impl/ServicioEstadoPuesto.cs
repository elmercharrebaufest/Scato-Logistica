using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios.Orquestador;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;

namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioEstadoPuesto : IServicioEstadoPuesto
    {
        private readonly ILogger log;
        private readonly IServicioRepositorio repositorio;
        private readonly IServicioNotificarUsuario notificar;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioComandos comandos;
        private readonly IConfiguracionProvider config;
        private IList<ConcentradorDto> puestos;

        public ServicioEstadoPuesto(ILogger log, IServicioRepositorio repositorio, IServicioNotificarUsuario notificar, 
            IServicioOrquestador orquestador, IServicioComandos comandos, IConfiguracionProvider config)
        {

            this.log = log;
            this.repositorio = repositorio;
            this.notificar = notificar;
            this.orquestador = orquestador;
            this.comandos = comandos;
            this.config = config;
            
            ActualizarPuestos();
        }
        public void ActualizarPuestos()
        {
            if (puestos != null)
            {
                foreach (var puesto in puestos)
                {
                    foreach (var sensor in puesto.Sensores)
                    {
                        try
                        {

                            var resultadoOrq = orquestador.CancelarSuscripcion(new ComandoCancelarSuscripcion
                            {
                                CodigoDispositivo = sensor.Codigo,
                                RutaAccesoSuscriptor = config.AppSettings["UrlNotificacionesWeb"],
                            });
                            if (resultadoOrq.Mensaje.Codigo != 0)
                            {
                                log.Error("No se pudo cancelar la suscripción para el lector {0}. Mensaje: {1}-{2}", sensor.Codigo,
                                    resultadoOrq.Mensaje.Codigo, resultadoOrq.Mensaje.Descripcion);
                            }
                        }
                        catch (Exception e)
                        {
                            log.Error(e, "No se pudo cancelar la suscripción para el lector {0}.", sensor.Codigo);
                        }
                    }
                }
            }
            puestos = new List<ConcentradorDto>();
            var listaDePuestos = repositorio.ListarPuestosDeBalanzasAutomaticas();
            foreach (var p in listaDePuestos.Where(x => !string.IsNullOrEmpty(x.Concentrador)))
            {
                var puesto = new ConcentradorDto()
                {
                    PuestoId = p.Id,
                    Concentrador = p.Concentrador
                };
                puesto.Sensores = orquestador.ListarSensoresPorConcentrador(p.Concentrador).Select(x => new Dominio.Dto.DispositivoGenericoDto { Codigo = x.Codigo, Descripcion = x.Descripcion }).ToList();
                if (puesto.Sensores.Any())
                {
                    foreach (var sensor in puesto.Sensores)
                    {
                        comandos.Ejecutar(new SuscribirDispositivos { Codigo = sensor.Codigo, Evento = "CambioEstadoSensor", RutaWeb = false });
                    }
                }
                puestos.Add(puesto);
            }
            log.Debug($"Total de puestos automaticos con sensores= {puestos.Count}");
        }

        public void NotificarCambioDeEstado(string sensor, string mensaje)
        {
            log.Debug($"Procesando notificaciones para {sensor} estado {mensaje}");
            var puesto = puestos.Where(x => x.Sensores.Any(y=>y.Codigo == sensor)).FirstOrDefault();
            if(puesto == null)
            {
                log.Error($"No hay puesto con contrador para el sensor: {sensor}");

                return;
            }            
            var estados = StringToByteArray(mensaje.Replace("-", ""));
            if(estados == null)
            {
                log.Error($"El byte de respuesta { mensaje } no corresponde con el de estado");
                return;
            }
            var byteEstado = estados[0];
            var estadoBalanza = new EstadoSensoresBalanzaDto()
            {
                PuestoId = puesto.PuestoId,
                BarreraEntradaActiva = !byteEstado.BitAt(7) && byteEstado.BitAt(6),
                BarreraSalidaActiva = !byteEstado.BitAt(5) && byteEstado.BitAt(4),
                SensorIngresoActiva = !byteEstado.BitAt(1),
                SensorTrompaActiva = !byteEstado.BitAt(0)
            };
            notificar.Notificar(new NotificacionDto
            {
                Grupo = "Automaticas",
                Mensaje = estadoBalanza.ToJson(),
                TipoAlerta = TipoAlerta.CambioEstadoBalanzas
            });
            puesto.EstadoSensoresBalanzaDto = estadoBalanza;

            //var mensajesCartel = repositorio.ObtenerMensajesCartelLed(CodigoMensajeCartelLed.BalanzaLimpiarCartelLed);

            //if (!estadoBalanza.SensorIngresoActiva)
            //    mensajesCartel = repositorio.ObtenerMensajesCartelLed(CodigoMensajeCartelLed.BalanzaAvanzarCamion);
            //else if (!estadoBalanza.SensorTrompaActiva)
            //    mensajesCartel = repositorio.ObtenerMensajesCartelLed(CodigoMensajeCartelLed.BalanzaRetrocederCamion);

            //mensajesCartel?.ToList().ForEach(x =>
            //{
            //    comandos.Ejecutar(new EnviarMensajeCarteLed()
            //    {
            //        Mensaje = x.Mensaje,
            //        PuestoDeTrabajoId = puesto.PuestoId,
            //        NumeroPrograma = x.Programa,
            //        NumeroTrama = x.Trama,
            //        NumeroVariable = x.Variable,
            //        SegundosDeEspera = x.SegundosDeEspera
            //    });
            //});
        }
        private byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        public bool ValidarEstadoPuesto(int puestoId)
        {
            var puesto = puestos.Where(x => x.PuestoId == puestoId ).FirstOrDefault();
            var valido = true;
            if (puesto == null)
            {
                log.Error($"No hay puesto con contrador para el puestoId: {puestoId}");
                return valido;
            }
            var mensajesCartel = repositorio.ObtenerMensajesCartelLed(CodigoMensajeCartelLed.BalanzaLimpiarCartelLed);

            if (!puesto.EstadoSensoresBalanzaDto.SensorIngresoActiva)
            {
                mensajesCartel = repositorio.ObtenerMensajesCartelLed(CodigoMensajeCartelLed.BalanzaAvanzarCamion);
                valido = false;
            }
            else if (!puesto.EstadoSensoresBalanzaDto.SensorTrompaActiva)
            {
                mensajesCartel = repositorio.ObtenerMensajesCartelLed(CodigoMensajeCartelLed.BalanzaRetrocederCamion);
                valido = false;
            }

            mensajesCartel?.ToList().ForEach(x =>
            {
                comandos.Ejecutar(new EnviarMensajeCarteLed()
                {
                    Mensaje = x.Mensaje,
                    PuestoDeTrabajoId = puesto.PuestoId,
                    NumeroPrograma = x.Programa,
                    NumeroTrama = x.Trama,
                    NumeroVariable = x.Variable,
                    SegundosDeEspera = x.SegundosDeEspera
                });
            });
            return valido;
        }
    }
}
