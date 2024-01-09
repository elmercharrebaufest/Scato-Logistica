using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Behavior;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Impl
{
    [AiErrorHandlerBehaviorAttribute]
    public class ServicioEstadoPuesto : IServicioEstadoPuesto
    {
        private readonly ILogger log;
        private readonly IServicioRepositorio repositorio;
        private readonly IServicioNotificarUsuario notificar;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioComandos comandos;
        private readonly IConfiguracionProvider config;
        private readonly ICache cache;
        //private IList<ConcentradorDto> puestos;

        public ServicioEstadoPuesto(ILogger log, IServicioRepositorio repositorio, IServicioNotificarUsuario notificar,
            IServicioOrquestador orquestador, IServicioComandos comandos, IConfiguracionProvider config, ICache cache)
        {
            this.log = log;
            this.repositorio = repositorio;
            this.notificar = notificar;
            this.orquestador = orquestador;
            this.comandos = comandos;
            this.config = config;
            this.cache = cache;

            if (cache.ObtenerTodos<ConcentradorDto>() == null || cache.ObtenerTodos<ConcentradorDto>().Count() == 0)
            {
                ActualizarPuestos();
            }
        }

        public void ActualizarPuestos()
        {
            var puestos = cache.ObtenerTodos<ConcentradorDto>();
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
            cache.RemoverPorGrupo("Puesto:");
            puestos = new List<ConcentradorDto>();
            var listaDePuestos = repositorio.ListarPuestosDeBalanzasAutomaticas();

            var sensoresSuscritos = new List<string>();
            foreach (var p in listaDePuestos.Where(x => x.ConfigSensores != null))
            {
                var puesto = new ConcentradorDto()
                {
                    PuestoId = p.Id,
                    Concentrador = p.ConfigSensores.Descripcion,
                    ConfigSensores = p.ConfigSensores,
                    Sensores = new List<DispositivoGenericoDto> {
                        new DispositivoGenericoDto {Codigo = p.ConfigSensores.SensorBarreraEntradaArriba },
                        new DispositivoGenericoDto {Codigo = p.ConfigSensores.SensorBarreraEntradaAbajo },
                        new DispositivoGenericoDto {Codigo = p.ConfigSensores.SensorPosicionIngreso },
                        new DispositivoGenericoDto {Codigo = p.ConfigSensores.SensorPosicionSalida},
                        new DispositivoGenericoDto {Codigo = p.ConfigSensores.SensorBarreraSalidaArriba },
                        new DispositivoGenericoDto {Codigo = p.ConfigSensores.SensorBarreraSalidaAbajo }
                    },
                    EstadoSensoresBalanzaDto = new EstadoSensoresBalanzaDto()
                };

                foreach (var sensor in puesto.Sensores)
                {
                    comandos.Ejecutar(new SuscribirDispositivos { Codigo = sensor.Codigo, Evento = "CambioEstadoSensor", RutaWeb = false });
                    sensoresSuscritos.Add(sensor.Codigo);
                }

                cache.Agregar($"Puesto:{puesto.PuestoId}", puesto);
                puestos.Add(puesto);
            }

            log.Debug($"Total de puestos automaticos con sensores= {puestos.Count}");
            ResuscribirGrupoBarrera(sensoresSuscritos);
        }

        public void NotificarCambioDeEstado(string sensor, string mensaje)
        {
            log.Debug($"Procesando notificaciones para {sensor} estado {mensaje}");
            var puestos = cache.ObtenerTodos<ConcentradorDto>();
            var puesto = puestos.Where(x => x.Sensores.Any(y => y.Codigo == sensor)).FirstOrDefault();
            if (puesto == null)
            {
                log.Info($"No hay puesto con contrador para el sensor: {sensor}");

                return;
            }
            var estados = StringToByteArray(mensaje.Replace("-", ""));
            if (estados == null)
            {
                log.Info($"El byte de respuesta {mensaje} no corresponde con el de estado");
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
            puesto.EstadoSensoresBalanzaDto = estadoBalanza;
        }

        public void NotificarCambioDeEstado(string sensor, bool mensaje)
        {
            var puestos = cache.ObtenerTodos<ConcentradorDto>();
            log.Debug($"Procesando notificaciones para {sensor} estado {mensaje}");
            var puesto = puestos.Where(x => x.Sensores.Any(y => y.Codigo == sensor)).FirstOrDefault();
            if (puesto == null)
            {
                log.Info($"No hay puesto con contrador para el sensor: {sensor}");

                return;
            }

            var estadoCambio = "";
            foreach (var propertyInfo in puesto.ConfigSensores.GetType().GetProperties())
            {
                var prop = propertyInfo.GetValue(puesto.ConfigSensores).ToString();
                if (prop == sensor)
                {
                    log.Debug($"Buscando {prop} igual a sensor {sensor} ");
                    if (propertyInfo.Name == "SensorBarreraEntradaArriba")
                    {
                        puesto.EstadoSensoresBalanzaDto.BarreraEntradaActiva = mensaje;
                        estadoCambio = "BarreraEntradaActiva";
                    }
                    if (propertyInfo.Name == "SensorBarreraEntradaAbajo")
                    {
                        puesto.EstadoSensoresBalanzaDto.BarreraEntradaDesactiva = mensaje;
                        estadoCambio = "BarreraEntradaActiva";
                    }
                    if (propertyInfo.Name == "SensorPosicionIngreso")
                    {
                        puesto.EstadoSensoresBalanzaDto.SensorIngresoActiva = mensaje;
                        estadoCambio = "SensorIngresoActiva";
                    }
                    if (propertyInfo.Name == "SensorPosicionSalida")
                    {
                        puesto.EstadoSensoresBalanzaDto.SensorTrompaActiva = mensaje;
                        estadoCambio = "SensorTrompaActiva";
                    }
                    if (propertyInfo.Name == "SensorBarreraSalidaArriba")
                    {
                        puesto.EstadoSensoresBalanzaDto.BarreraSalidaActiva = mensaje;
                        estadoCambio = "BarreraSalidaActiva";
                    }
                    if (propertyInfo.Name == "SensorBarreraSalidaAbajo")
                    {
                        puesto.EstadoSensoresBalanzaDto.BarreraSalidaDesactiva = mensaje;
                        estadoCambio = "BarreraSalidaActiva";
                    }
                }
            }

            var estadoBalanza = new EstadoSensoresBalanzaDto()
            {
                PuestoId = puesto.PuestoId,
                BarreraEntradaActiva = puesto.EstadoSensoresBalanzaDto.BarreraEntradaActiva && !puesto.EstadoSensoresBalanzaDto.BarreraEntradaDesactiva,
                BarreraSalidaActiva = puesto.EstadoSensoresBalanzaDto.BarreraSalidaActiva && !puesto.EstadoSensoresBalanzaDto.BarreraSalidaDesactiva,
                SensorIngresoActiva = puesto.EstadoSensoresBalanzaDto.SensorIngresoActiva,
                SensorTrompaActiva = puesto.EstadoSensoresBalanzaDto.SensorTrompaActiva,
                SensorModificado = estadoCambio
            };
            notificar.Notificar(new NotificacionDto
            {
                Grupo = "Automaticas",
                Mensaje = estadoBalanza.ToJson(),
                TipoAlerta = TipoAlerta.CambioEstadoBalanzas
            });
            puesto.EstadoSensoresBalanzaDto = estadoBalanza;
            cache.Remover($"Puesto:{puesto.PuestoId}");
            cache.Agregar($"Puesto:{puesto.PuestoId}", puesto);
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
            var puesto = cache.Obtener<ConcentradorDto>($"Puesto:{puestoId}");
            var valido = true;
            if (puesto == null)
            {
                log.Error($"No hay puesto con contrador para el puestoId: {puestoId}");
                return valido;
            }
            log.Debug($"Se encontro puesto Id {puesto.PuestoId}");
            var mensajesCartel = repositorio.ListarMensajesCartelLed(CodigoMensajeCartelLed.BalanzaLimpiarCartelLed);
            var estadoEntradaArriba = orquestador.Ejecutar(new EjecutarConsultaSensor { CodigoDispositivo = puesto.ConfigSensores.SensorBarreraEntradaArriba }) as ResultadoEstadoSensor;
            var estadoEntradaAbajo = orquestador.Ejecutar(new EjecutarConsultaSensor { CodigoDispositivo = puesto.ConfigSensores.SensorBarreraEntradaAbajo }) as ResultadoEstadoSensor;
            var estadoSalidaArriba = orquestador.Ejecutar(new EjecutarConsultaSensor { CodigoDispositivo = puesto.ConfigSensores.SensorBarreraSalidaArriba }) as ResultadoEstadoSensor;
            var estadoSalidaAbajo = orquestador.Ejecutar(new EjecutarConsultaSensor { CodigoDispositivo = puesto.ConfigSensores.SensorBarreraSalidaAbajo }) as ResultadoEstadoSensor;
            if (!estadoEntradaAbajo.EstadoActivo && estadoEntradaArriba.EstadoActivo
                && !estadoSalidaAbajo.EstadoActivo && estadoSalidaArriba.EstadoActivo)
            {
                log.Debug($"Algunas de las dos barreras no estan cerradas.");
                return false;
            }
            var estadoSensorIngreso = orquestador.Ejecutar(new EjecutarConsultaSensor { CodigoDispositivo = puesto.ConfigSensores.SensorPosicionIngreso }) as ResultadoEstadoSensor;
            var estadoSensorTrompa = orquestador.Ejecutar(new EjecutarConsultaSensor { CodigoDispositivo = puesto.ConfigSensores.SensorPosicionSalida }) as ResultadoEstadoSensor;
            log.Debug($"Estado del sensor de entrada: {estadoSensorIngreso.EstadoActivo}, Estado del sensor de salida: {estadoSensorTrompa.EstadoActivo}, ");

            if (!estadoSensorIngreso.EstadoActivo)
            {
                mensajesCartel = repositorio.ListarMensajesCartelLed(CodigoMensajeCartelLed.BalanzaAvanzarCamion);
                valido = false;
            }
            else if (!estadoSensorTrompa.EstadoActivo)
            {
                mensajesCartel = repositorio.ListarMensajesCartelLed(CodigoMensajeCartelLed.BalanzaRetrocederCamion);
                valido = false;
            }

            mensajesCartel?.ToList().ForEach(x =>
            {
                comandos.Ejecutar(new EnviarMensajeCartelLed()
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

        public void NotificarSensorBarrera(NotificacionEvento notificacion)
        {
            log.Debug($"Procesando notificaciones para {notificacion?.CodigoDispositivo} CodigoEvento {notificacion?.CodigoEvento}");
            var sensor = notificacion?.CodigoDispositivo ?? string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(sensor))
                {
                    var listadoSensores = repositorio.ListarSensoresBarrerasActivos().ToList();

                    var sensoresArriba = listadoSensores
                        .Where(w => w.CodigoDispositivoSensorArriba == sensor)
                        .Select(s => new EstadoSensorDto { Id = s.Id, GrupoId = s.VisualizacionBarrera.Id, Barrera = s.Barrera, Estado = bool.Parse(notificacion.Datos.ContainsKey("Mensaje") ? notificacion.Datos["Mensaje"] : string.Empty) }).ToList();

                    var sensoresAbajo = listadoSensores
                        .Where(w => w.CodigoDispositivoSensorAbajo == sensor)
                        .Select(s => new EstadoSensorDto { Id = s.Id, GrupoId = s.VisualizacionBarrera.Id, Barrera = s.Barrera, Estado = bool.Parse(notificacion.Datos.ContainsKey("Mensaje") ? notificacion.Datos["Mensaje"] : string.Empty) }).ToList();

                    var notificacionSensorBarrera = new EstadoSensoresBarreraDto
                    {
                        Dispositivo = sensor,
                        SensoresArriba = sensoresArriba,
                        SensoresAbajo = sensoresAbajo
                    };

                    notificar.Notificar(new NotificacionDto
                    {
                        Grupo = "SENSORESBARRERA",
                        Mensaje = notificacionSensorBarrera.ToJson(),
                        TipoAlerta = TipoAlerta.CambioEstadoBarrera
                    });
                }
            }
            catch (Exception e)
            {
                log.Error("Notificar cambio sensor vagones error no controlado sensor: {0}, detalle del error : {1}", sensor, e);
            }
        }

        public void NotificarEstado()
        {
            log.Debug($"Actualizando el estado de los puestos");
            var puestos = cache.ObtenerTodos<ConcentradorDto>();
            var listaSensores = new List<string>();
            foreach (var puesto in puestos)
            {
                listaSensores = puesto.Sensores.Select(x => x.Codigo).ToList();
            }
            foreach (var sensor in listaSensores)
            {
                orquestador.Ejecutar(new EjecutarNotificacionEstadoSensor { CodigoDispositivo = sensor });
            }
        }

        //Para refactor por cache o base
        public IList<ConcentradorDto> ConsultarEstadoBarreras()
        {
            return cache.ObtenerTodos<ConcentradorDto>();
        }

        public void ActualizarBarreras(string nombrePc)
        {
            var sensores = repositorio.ListarSensoresBarrerasActivosPorNombreDePC(nombrePc);
            if (sensores != null)
            {
                foreach (var sensor in sensores)
                {
                    orquestador.Ejecutar(new EjecutarNotificacionEstadoSensor { CodigoDispositivo = sensor.CodigoDispositivoSensorAbajo });
                    orquestador.Ejecutar(new EjecutarNotificacionEstadoSensor { CodigoDispositivo = sensor.CodigoDispositivoSensorArriba });
                }
            }
        }

        private void ResuscribirGrupoBarrera(List<string> sensoresSuscritos)
        {
            var configuraciones = orquestador.ListarGruposBarrera();
            var gruposBarreraCodigos = configuraciones.Select(q => q.Codigo).ToList();
            var grupoBarreraActivas = new List<GrupoBarreraDto>();
            foreach (var grupoBarrera in gruposBarreraCodigos)
            {
                var grupoBarreraDatos = orquestador.ObtenerConfiguracionGrupoBarrera(grupoBarrera);
                grupoBarreraActivas.Add(grupoBarreraDatos);
                try
                {
                    if (!sensoresSuscritos.Contains(grupoBarreraDatos.SensorAbajoCodigo))
                        CancelarSuscripcion(grupoBarreraDatos.SensorAbajoCodigo);
                    if (!sensoresSuscritos.Contains(grupoBarreraDatos.SensorAbajoCodigo))
                        CancelarSuscripcion(grupoBarreraDatos.SensorAbajoCodigo);
                    if (!sensoresSuscritos.Contains(grupoBarreraDatos.SensorAbajoCodigo))
                        CancelarSuscripcion(grupoBarreraDatos.SensorAbajoCodigo);
                }
                catch (Exception e)
                {
                    log.Error(e, "No se pudo cancelar la suscripción para el el grupo {0}.", grupoBarreraDatos.AgrupadorCodigo);
                }
            }

            var gruposEnUso = repositorio.ObtenerGruposBarreraEnUso(gruposBarreraCodigos).Distinct();
            foreach (var grupo in gruposEnUso)
            {
                var grupoBarreraASuscribir = grupoBarreraActivas.FirstOrDefault(q => q.AgrupadorCodigo == grupo);
                if (!sensoresSuscritos.Contains(grupoBarreraASuscribir?.SensorArribaCodigo))
                    comandos.Ejecutar(new SuscribirDispositivos { Codigo = grupoBarreraASuscribir.SensorArribaCodigo, Evento = "CambioEstadoSensor", RutaWeb = false });
                if (!sensoresSuscritos.Contains(grupoBarreraASuscribir.SensorAbajoCodigo))
                    comandos.Ejecutar(new SuscribirDispositivos { Codigo = grupoBarreraASuscribir.SensorAbajoCodigo, Evento = "CambioEstadoSensor", RutaWeb = false });
                if (!sensoresSuscritos.Contains(grupoBarreraASuscribir.SensorSegundoCruceCodigo))
                    comandos.Ejecutar(new SuscribirDispositivos { Codigo = grupoBarreraASuscribir.SensorSegundoCruceCodigo, Evento = "CambioEstadoSensor", RutaWeb = false });
            }
        }

        private void CancelarSuscripcion(string codigoSensor)
        {
            var resultadoCancelacion = orquestador.CancelarSuscripcion(new ComandoCancelarSuscripcion
            {
                CodigoDispositivo = codigoSensor,
                RutaAccesoSuscriptor = config.AppSettings["UrlNotificacionesWeb"],
            });

            if (resultadoCancelacion.Mensaje.Codigo != 0)
            {
                log.Error("No se pudo cancelar la suscripción para el lector {0}. Mensaje: {1}-{2}", codigoSensor,
                    resultadoCancelacion.Mensaje.Codigo, resultadoCancelacion.Mensaje.Descripcion);
            }
        }

        public void NotificarSensorBarreraHidraulicas(NotificacionEvento notificacion)
        {
            log.Debug($"Procesando notificaciones para {notificacion?.CodigoDispositivo} CodigoEvento {notificacion?.CodigoEvento}");
            var sensor = notificacion?.CodigoDispositivo ?? string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(sensor))
                {
                    var listadoSensores = repositorio.ListarSensoresBarrerasHidraulicasActivos();

                    var sensoresArriba = listadoSensores
                        .Where(w => w.CodigoDispositivoSensorArriba == sensor)
                        .Select(s => new EstadoSensorDto
                        {
                            Id = s.Id,
                            GrupoId = s.VisualizacionBarrera.Id,
                            Barrera = s.Barrera,
                            Estado = bool.Parse(notificacion.Datos.ContainsKey("Mensaje") ? notificacion.Datos["Mensaje"] : string.Empty),
                            PuestoDeTrabajoId = s.PuestoDeTrabajoId
                        }).ToList();

                    var sensoresAbajo = listadoSensores
                        .Where(w => w.CodigoDispositivoSensorAbajo == sensor)
                        .Select(s => new EstadoSensorDto
                        {
                            Id = s.Id,
                            GrupoId = s.VisualizacionBarrera.Id,
                            Barrera = s.Barrera,
                            Estado = bool.Parse(notificacion.Datos.ContainsKey("Mensaje") ? notificacion.Datos["Mensaje"] : string.Empty),
                            PuestoDeTrabajoId = s.PuestoDeTrabajoId
                        }).ToList();

                    var notificacionSensorBarrera = new EstadoSensoresBarreraDto
                    {
                        Dispositivo = sensor,
                        SensoresArriba = sensoresArriba,
                        SensoresAbajo = sensoresAbajo
                    };

                    notificar.Notificar(new NotificacionDto
                    {
                        Grupo = Dominio.Constantes.NotificacionGrupos.SensoresBarreraHidraulica,
                        Mensaje = notificacionSensorBarrera.ToJson(),
                        TipoAlerta = TipoAlerta.CambioEstadoBarreraHidraulica
                    });
                }
            }
            catch (Exception e)
            {
                log.Error("Notificar cambio sensor vagones error no controlado sensor: {0}, detalle del error : {1}", sensor, e);
            }
        }
    }
}