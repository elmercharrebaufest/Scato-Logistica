using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.HealthCheck;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios.Behavior;
using Molinos.Scato.Servicios.Interfaces;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Servicios.Impl
{
    [AiErrorHandlerBehaviorAttribute]
    public class ServicioLlamadoAutomatico : IServicioLlamadoAutomatico
    {
        private readonly IServicioRepositorio repositorio;
        private readonly ILogger log;
        private readonly IServicioComandos comandos;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioHealthCheck healthCheckService;
        private readonly IServicioNotificarUsuario notificarUsuario;

        public ServicioLlamadoAutomatico(IServicioRepositorio repositorio, ILogger log, IServicioComandos comandos, IServicioOrquestador orquestador, IServicioHealthCheck healthCheckService, IServicioNotificarUsuario notificarUsuario)
        {
            this.repositorio = repositorio;
            this.log = log;
            this.comandos = comandos;
            this.orquestador = orquestador;
            this.healthCheckService = healthCheckService;
            this.notificarUsuario = notificarUsuario;
        }

        public void Llamar(LlamadoAutomatico tipoLlamadoAutomatico)
        {
            switch (tipoLlamadoAutomatico)
            {
                case LlamadoAutomatico.Granos:
                    DetenerLlamadoAutomaticoGranos();
                    LlamarAutomaticoGranos();
                    break;

                case LlamadoAutomatico.NoGranos:
                    LlamarAutomaticoNoGranos();
                    break;
            }
        }

        public void SincronizarMOAPayEstadoDePagos()
        {
            var fechaDesde = repositorio.ObtenerUltimaFechaDePagoTasaMunicipal();
            comandos.Ejecutar(new MOAPaySincronizarEstadoDePagos
            {
                Disponible = MOAPay.Filtros.SI,
                Pagado = MOAPay.Filtros.SI,
                TipoFecha = MOAPay.Filtros.FECHAPAGO,
                FechaDesde = fechaDesde ?? DateTime.Now.AddDays(-30),
                FechaHasta = DateTime.Now,
            });
        }

        public void SincronizarMOAPayCPE()
        {
            var cpeList = repositorio.ListarCPEFiltradasPorFechaDeCacheado(DateTime.Now.AddHours(-1), DateTime.Now);

            foreach (var cpe in cpeList)
            {
                var tipoDeVehiculo = repositorio.ObtenerTipodVehiculoPorPesoBruto(cpe.PesoBruto, 5);
                comandos.Ejecutar(new MOAPayCrearModificarCPE
                {
                    NumeroDocumento = cpe.NroCtg.ToString(),
                    Dominio = cpe.Dominio,
                    TipoDeVehiculo = ObtenerMOAPayTipoVehiculo(tipoDeVehiculo),
                    CuitInterviniente = cpe.CuitTransportista.ToString()
                });
            }
        }

        public async Task<HealthCheckResult> EjecutarHealthCheckAsync(string jobName)
        {
            var result = new HealthCheckResult();
            try
            {
                var servicioExterno = repositorio.ObtenerMonitoreoServicioExternoPorJob(jobName);
                if (servicioExterno == null)
                {
                    result.Message = $"No se encontro ExternalService configurado para {jobName}";
                    return result;
                }

                result = await healthCheckService.CheckAsync(servicioExterno, CancellationToken.None);
                var comando = new ModificarMonitoreoServicioExterno
                {
                    Id = servicioExterno.Id,
                    UltimoEstado = result.Status,
                    UltimaVerificacion = DateTime.Now,
                };
                comandos.Ejecutar(comando);

                var payload = new
                {
                    key = servicioExterno.KeyJob,
                    status = result.Status.ToString(),
                    ultimaVerificacion = comando.UltimaVerificacion.ToString(),
                };
                var notificacion = new NotificacionDto
                {
                    Hora = DateTime.Now,
                    Grupo = Constantes.NotificacionGrupos.EstadoServicioExterno,
                    Mensaje = Newtonsoft.Json.JsonConvert.SerializeObject(payload),
                    TipoAlerta = TipoAlerta.NotificacionEstadoWeb
                };
                notificarUsuario.NotificarEstadoServicioExterno(notificacion);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        private void LlamarAutomaticoGranos()
        {
            var configuracionGeneral = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);
            if (configuracionGeneral == null
                || string.IsNullOrEmpty(configuracionGeneral.Valor)
                || !bool.TryParse(configuracionGeneral.Valor, out bool automatismoGranoGeneral)
                || !automatismoGranoGeneral)
                return;

            var callesPreHidraulica = repositorio.ListarCallesPorTipo(TipoCalle.PlayaInterna).Where(x => !x.Deshabilitada);
            foreach (var calle in callesPreHidraulica)
            {
                if (!repositorio.ExisteEspacioDisponibleParaLlamarEnCartel(CodigoMensajeCartelLed.LlamadoCallePreBalanza))
                    break;

                ValidarTipoLlamadoAutomaticoGrano(calle);
            }
        }

        private void ValidarTipoLlamadoAutomaticoGrano(CalleDto callePH)
        {
            if (callePH.AutomatismoTipoLlamado.Codigo == AutomatismoTipoLlamado.PaseDirecto)
                ValidarLlamadoPorPasoDirecto(callePH);
            else if (callePH.AutomatismoTipoLlamado.Codigo == AutomatismoTipoLlamado.UnoAUno)
                ValidarLlamadoPor1A1(callePH);
            else if (callePH.AutomatismoTipoLlamado.Codigo == AutomatismoTipoLlamado.PorFila)
                ValidarLlamadoPorFila(callePH);
        }

        private void ValidarLlamadoPorPasoDirecto(CalleDto callePH)
        {
            if (repositorio.ExisteLlamadoCallePreBalanzaPorTipoDeLlamado(callePH.Id, callePH.AutomatismoTipoLlamado.Codigo))
                return;

            if (!repositorio.ValidarEspacioDisponibleEnCallePreHidraulica(callePH.Id))
                return;

            var configuracion = repositorio.ListarAutomatismoGrano().Where(x => x.Activo && x.CallePreHidraulicaId == callePH.Id).FirstOrDefault();
            if (configuracion == null)
                return;

            if (!repositorio.ExisteCamionesEnCalle(configuracion.CallePreBalanzaId))
                return;

            if (repositorio.EstaDisponibleCalle(configuracion.CallePreBalanzaId))
                LlamarCallePreBalanza(configuracion);
        }

        private void ValidarLlamadoPorFila(CalleDto callePH)
        {
            var configuraciones = repositorio.ListarAutomatismoGrano().Where(x => x.Activo && x.CallePreHidraulicaId == callePH.Id);
            var configuracion = ObtenerAutomatismoGranoLlamadoPorFila(configuraciones);
            if (configuracion == null)
                return;

            var cantidadCamionesEnFilaPB = repositorio.ListarCallePorRecorridoPorCalleId(configuracion.CallePreBalanzaId).Count();
            var cantidadCamionesEnFilaPH = repositorio.ObtenerCantidadCamionesEnCallePreHidraulica(configuracion.CallePreHidraulicaId);
            var tieneEspacioSuficiente = callePH.CantidadDeCamiones - cantidadCamionesEnFilaPH >= cantidadCamionesEnFilaPB;
            if (tieneEspacioSuficiente)
                LlamarCallePreBalanza(configuracion);
        }

        private AutomatismoGranoDto ObtenerAutomatismoGranoLlamadoPorFila(IEnumerable<AutomatismoGranoDto> configuraciones)
        {
            Dictionary<int, DateTime> fechaIngresoPrimerCamionPorFilaPB = new Dictionary<int, DateTime>();
            foreach (var configuracion in configuraciones)
            {
                var callePrebalanza = repositorio.ObtenerCalle(configuracion.CallePreBalanzaId);
                if (callePrebalanza.Deshabilitada || callePrebalanza.Bloqueada || callePrebalanza.FechaLLamada != null)
                    continue;

                var camionesEnCallePB = repositorio.ListarCallePorRecorridoPorCalleId(configuracion.CallePreBalanzaId);
                if (camionesEnCallePB.Count() == callePrebalanza.CantidadDeCamiones)
                {
                    var fechaIngresoPrimerCamion = camionesEnCallePB.OrderBy(x => x.FechaIngeso).Select(x => x.FechaIngeso).FirstOrDefault();
                    fechaIngresoPrimerCamionPorFilaPB.Add(callePrebalanza.Id, fechaIngresoPrimerCamion);
                }
            }

            return fechaIngresoPrimerCamionPorFilaPB.Any()
                ? configuraciones.Where(x => fechaIngresoPrimerCamionPorFilaPB.Any(q => q.Key == x.CallePreBalanzaId))
                                .OrderBy(x => fechaIngresoPrimerCamionPorFilaPB[x.CallePreBalanzaId])
                                .FirstOrDefault()
                : null;
        }

        private void LlamarCallePreBalanza(AutomatismoGranoDto configuracion)
        {
            var resultadoCrearCallePreBalanzaPlayaInterna = comandos.Ejecutar(new CrearCallePreBalanzaPlayaInterna
            {
                CallePlayaInternaId = configuracion.CallePreHidraulicaId,
                CallePreBalanzaId = configuracion.CallePreBalanzaId,
                CodigoAutomatismoTipoLlamado = configuracion.CodigoAutomatismoTipoLlamado,
            });
            if (resultadoCrearCallePreBalanzaPlayaInterna.HayErrores)
                return;

            var resultadoInsertarCalleCartelLed = comandos.Ejecutar(new InsertarSlotMensajeCartelLed()
            {
                Codigo = CodigoMensajeCartelLed.LlamadoCallePreBalanza,
                CalleId = configuracion.CallePreBalanzaId,
            }) as ResultadoMensajeCartelLed;
            if (!resultadoInsertarCalleCartelLed.HayErrores && resultadoInsertarCalleCartelLed.ListaDeMensajes.Any())
            {
                EnviarMensajesAlCartel(resultadoInsertarCalleCartelLed.ListaDeMensajes);
            }
        }

        private void ValidarLlamadoPor1A1(CalleDto callePH)
        {
            if (!repositorio.ValidarEspacioDisponibleEnCallePreHidraulica(callePH.Id))
                return;

            var callesPBConAutomatismo = repositorio
                .ListarAutomatismoGrano()
                .Where(x => x.Activo && x.CallePreHidraulicaId == callePH.Id)
                .Select(c => c.CallePreBalanzaId)
                .ToList();

            var recorridoALlamar = repositorio.ObtenerPrimerRecorridosDisponibleParaLlamadoAutomaticoGranos(callesPBConAutomatismo);

            if (recorridoALlamar != null)
            {
                LlamarCamionPreBalanza(callePH.Id, recorridoALlamar, esCamionEnEspera: false);

                var camionEnEspera = repositorio.ObtenerCamionEnEsperaLlamadoGranos(recorridoALlamar.CalleId);

                if (camionEnEspera != null)
                {
                    LlamarCamionPreBalanza(callePH.Id, camionEnEspera, esCamionEnEspera: true);
                }
            }
        }

        private void LlamarCamionPreBalanza(int callePHId, CallePorRecorridoDto camion, bool esCamionEnEspera)
        {
            var resultadoCrearCallePreBalanzaPlayaInterna = comandos.Ejecutar(new CrearCallePreBalanzaPlayaInterna
            {
                CallePlayaInternaId = callePHId,
                CallePreBalanzaId = camion.CalleId,
                CodigoAutomatismoTipoLlamado = Constantes.AutomatismoTipoLlamado.UnoAUno,
                RecorridoId = camion.RecorridoId,
                EsCamionEnEspera = esCamionEnEspera,
            });

            if (resultadoCrearCallePreBalanzaPlayaInterna.HayErrores)
                return;

            var resultadoInsertarCalleCartelLed = comandos.Ejecutar(new InsertarSlotMensajeCartelLed()
            {
                Codigo = esCamionEnEspera ? CodigoMensajeCartelLed.LlamadoCamionPreBalanza : CodigoMensajeCartelLed.LlamadoCallePreBalanza,
                CalleId = camion.CalleId,
                EsLlamadoPorCamion = true,
                EsCamionEnEspera = esCamionEnEspera,
                RecorridoId = camion.RecorridoId,
            }) as ResultadoMensajeCartelLed;

            if (!resultadoInsertarCalleCartelLed.HayErrores && resultadoInsertarCalleCartelLed.ListaDeMensajes.Any())
                EnviarMensajesAlCartel(resultadoInsertarCalleCartelLed.ListaDeMensajes);
        }

        private void EnviarMensajesAlCartel(List<MensajeCartelLedDto> listaDeMensajes)
        {
            var cartel = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoPlayaInterna, Constantes.ConfiguracionGeneral.PreBalanza.CartelLedPreBalanza);

            foreach (var mensajeCartelLed in listaDeMensajes)
            {
                orquestador.Ejecutar(new EjecutarEnviarMensaje
                {
                    Texto = mensajeCartelLed.HistorialMensajeCartelLed?.Mensaje ?? "-",
                    CodigoDispositivo = cartel?.Valor,
                    NumeroTrama = mensajeCartelLed.Trama,
                    NumeroPrograma = mensajeCartelLed.Programa,
                    NumeroVariable = mensajeCartelLed.Variable,
                });
            }
        }

        private void DetenerLlamadoAutomaticoGranos()
        {
            var callesPreBalanzaPlayaInterna = repositorio.ListarCallePreBalanzaLlamadasPorAutomatismo();
            foreach (var callePreBalanzaPlayaInterna in callesPreBalanzaPlayaInterna)
                ValidarTipoLiberarAutomaticoGrano(callePreBalanzaPlayaInterna);
        }

        private void ValidarTipoLiberarAutomaticoGrano(CallePreBalanzaPlayaInternaDto callePreBalanzaPlayaInterna)
        {
            if (callePreBalanzaPlayaInterna.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.PaseDirecto)
                ValidarLiberarPorPaseDirecto(callePreBalanzaPlayaInterna);
            else if (callePreBalanzaPlayaInterna.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.UnoAUno)
                ValidarLiberarPor1A1(callePreBalanzaPlayaInterna);
            else if (callePreBalanzaPlayaInterna.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.PorFila)
                ValidarLiberarPorFila(callePreBalanzaPlayaInterna);
            else
                ValidarLiberarPorFila(callePreBalanzaPlayaInterna);
        }

        private void ValidarLiberarPorPaseDirecto(CallePreBalanzaPlayaInternaDto callePreBalanzaPlayaInterna)
        {
            int cantidadCamionesEnCallePreBalanza = repositorio.ListarCallePorRecorridoPorCalleId(callePreBalanzaPlayaInterna.CallePreBalanza.Id).Count();
            int cantidadCamionesEnCallePlayaInterna = repositorio.ListarCallePorRecorridoPorCalleId(callePreBalanzaPlayaInterna.CallePlayaInterna.Id).Count();

            bool callePreHidraulicaLlena = cantidadCamionesEnCallePlayaInterna >= callePreBalanzaPlayaInterna.CallePlayaInterna.CantidadDeCamiones;
            bool callePreBalanzaVacia = cantidadCamionesEnCallePreBalanza == 0;

            if (callePreHidraulicaLlena || callePreBalanzaVacia)
                LiberarCallePreBalanza(callePreBalanzaPlayaInterna);
        }

        private void ValidarLiberarPorFila(CallePreBalanzaPlayaInternaDto callePreBalanzaPlayaInterna)
        {
            var camionesEnCalle = repositorio.ListarCallePorRecorridoPorCalleId(callePreBalanzaPlayaInterna.CallePreBalanza.Id);
            if (!camionesEnCalle.Any())
                LiberarCallePreBalanza(callePreBalanzaPlayaInterna);
        }

        private void LiberarCallePreBalanza(CallePreBalanzaPlayaInternaDto callePreBalanzaPlayaInterna)
        {
            var resultadoEliminarCallePreBalanzaPlayaInterna = comandos.Ejecutar(new EliminarCallePreBalanzaPlayaInterna()
            {
                CallePlayaInternaId = callePreBalanzaPlayaInterna.CallePlayaInterna.Id,
                CallePreBalanzaId = callePreBalanzaPlayaInterna.CallePreBalanza.Id,
                CodigoAutomatismoTipoLlamado = callePreBalanzaPlayaInterna.CodigoAutomatismoTipoLlamado
            });
            if (resultadoEliminarCallePreBalanzaPlayaInterna.HayErrores)
                return;

            var resultadoLimpiarCalleCartelLed = comandos.Ejecutar(new LimpiarHistorialMensajeCartelLed()
            {
                Codigo = CodigoMensajeCartelLed.LlamadoCallePreBalanza,
                CalleId = callePreBalanzaPlayaInterna.CallePreBalanza.Id
            }) as ResultadoMensajeCartelLedReordenado;

            if (!resultadoLimpiarCalleCartelLed.HayErrores && resultadoLimpiarCalleCartelLed.ListaDeMensajes.Any())
                EnviarMensajesAlCartel(resultadoLimpiarCalleCartelLed.ListaDeMensajes);
        }

        private void ValidarLiberarPor1A1(CallePreBalanzaPlayaInternaDto callePreBalanzaPlayaInterna)
        {
            var camionLlamado = repositorio.ObtenerCallePorRecorrido(callePreBalanzaPlayaInterna.CallePreBalanza.Id, callePreBalanzaPlayaInterna.RecorridoId.Value);
            if (camionLlamado == null || !camionLlamado.FechaEgreso.HasValue)
                return;

            LiberarCamionPreBalanza(callePreBalanzaPlayaInterna);
            var camionEnEspera = repositorio.ObtenerCallePreBalanzaPlayaInternaDeCamionEnEspera(callePreBalanzaPlayaInterna.CallePreBalanza.Id);
            if (camionEnEspera != null)
                LiberarCamionPreBalanza(camionEnEspera);
        }

        private void LiberarCamionPreBalanza(CallePreBalanzaPlayaInternaDto callePreBalanzaPlayaInterna)
        {
            var resultadoEliminarCallePreBalanzaPlayaInterna = comandos.Ejecutar(new EliminarCallePreBalanzaPlayaInterna()
            {
                CallePlayaInternaId = callePreBalanzaPlayaInterna.CallePlayaInterna.Id,
                CodigoAutomatismoTipoLlamado = callePreBalanzaPlayaInterna.CodigoAutomatismoTipoLlamado,
                RecorridoId = callePreBalanzaPlayaInterna.RecorridoId,
            });

            if (resultadoEliminarCallePreBalanzaPlayaInterna.HayErrores)
                return;

            var resultadoLimpiarCalleCartelLed = comandos.Ejecutar(new LimpiarHistorialMensajeCartelLed()
            {
                Codigo = callePreBalanzaPlayaInterna.EsCamionEnEspera ? CodigoMensajeCartelLed.LlamadoCamionPreBalanza : CodigoMensajeCartelLed.LlamadoCallePreBalanza,
                CalleId = callePreBalanzaPlayaInterna.CallePreBalanza.Id,
                LimpiarCamion = true,
                RecorridoId = callePreBalanzaPlayaInterna.RecorridoId,
            }) as ResultadoMensajeCartelLedReordenado;

            if (!resultadoLimpiarCalleCartelLed.HayErrores && resultadoLimpiarCalleCartelLed.ListaDeMensajes.Any())
                EnviarMensajesAlCartel(resultadoLimpiarCalleCartelLed.ListaDeMensajes);
        }

        private void LlamarAutomaticoNoGranos()
        {
            //validar activo general de automatismo
            var configuracionGeneral = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoPuerto, Constantes.ConfiguracionGeneral.LlamadoAutomatico.NoGranos);
            if (configuracionGeneral == null
                || string.IsNullOrEmpty(configuracionGeneral.Valor)
                || !bool.TryParse(configuracionGeneral.Valor, out bool automatismoGranoGeneral)
                || !automatismoGranoGeneral)
            {
                this.log.Info(string.Format("El automatismo está desactivado"));
                return;
            }

            var recorridos = repositorio.ObtenerPrimerosRecorridosDisponibleParaLlamadoAutomaticoNoGranos();

            var automatismosNoGranos = repositorio.ListarAutomatismoNoGrano();

            foreach (var recorrido in recorridos)
            {
                var cartelLed = repositorio.ObtenerCartelDisponible(CodigoMensajeCartelLed.LlamadoCamionNoGrano);

                if (cartelLed == null)
                    break;

                var asignacionNoGranoEnRecorrido = repositorio.ObtenerAsignacionNoGranoEnRecorridoPorRecorridoId(recorrido.Id);

                if (asignacionNoGranoEnRecorrido == null)
                {
                    this.log.Info(string.Format("no se encontro el recorrido {0} en la tabla asignacionNoGranoEnRecorrido, por lo que no se pudo obtener su calle planta", recorrido.Id));
                    continue;
                }

                var hayAutomatismoParaLlamado = automatismosNoGranos.Exists(x => x.ActivoLlamado && x.CallePlantaId == asignacionNoGranoEnRecorrido.CallePlantaId);

                if (!hayAutomatismoParaLlamado)
                {
                    this.log.Info(string.Format("no hay un automatismo con llamado activo para el recorrido {0} que tiene callePlantaId {1}", recorrido.Id, asignacionNoGranoEnRecorrido.CallePlantaId));
                    continue;
                }

                var lugaresDisponibles = repositorio.ObtenerDisponibilidadEnCallePlantaNoGranos((int)asignacionNoGranoEnRecorrido.CallePlantaId);

                if (lugaresDisponibles > 0)
                {
                    var callePorRecorridoNoGranos = repositorio.ObtenerCallePorRecorridoPlayaExternaNoGranosPorRecorridoId(recorrido.Id);

                    var resultadoInsertarCalleCartelLed = comandos.Ejecutar(new InsertarSlotMensajeCartelLed()
                    {
                        Codigo = CodigoMensajeCartelLed.LlamadoCamionNoGrano,
                        CalleId = callePorRecorridoNoGranos.CalleId,
                        EsLlamadoPorCamion = true,
                        RecorridoId = recorrido.Id,
                        EsCamionEnEspera = false
                    }) as ResultadoMensajeCartelLed;

                    if (!resultadoInsertarCalleCartelLed.HayErrores)
                    {
                        var cartel = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePostCalado, Constantes.ConfiguracionGeneral.PostCalado.CartelLedPostCalado);

                        comandos.Ejecutar(new EnviarMensajeCartelLed
                        {
                            Mensaje = recorrido.Patente,
                            Codigo = cartel?.Valor,
                            NumeroTrama = resultadoInsertarCalleCartelLed.NumeroTrama,
                            NumeroPrograma = resultadoInsertarCalleCartelLed.NumeroPrograma,
                            NumeroVariable = resultadoInsertarCalleCartelLed.NumeroVariable,
                            SegundosDeEspera = resultadoInsertarCalleCartelLed.SegundosDeEspera,
                        });
                    }
                }
            }
        }

        private string ObtenerMOAPayTipoVehiculo(TipoVehiculo tipoVehiculo)
        {
            switch (tipoVehiculo)
            {
                case TipoVehiculo.Bitren:
                case TipoVehiculo.CamiónC:
                case TipoVehiculo.CamiónD:
                case TipoVehiculo.CamiónE:
                    return MOAPay.TipoDeVehiculo.ESCALABLE;

                default:
                    return MOAPay.TipoDeVehiculo.COMUN;
            }
        }
    }
}