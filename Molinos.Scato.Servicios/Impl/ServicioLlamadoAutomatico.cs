using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios.Behavior;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Impl
{
    [AiErrorHandlerBehaviorAttribute]
    public class ServicioLlamadoAutomatico : IServicioLlamadoAutomatico
    {
        private readonly IServicioRepositorio repositorio;
        private readonly ILogger log;
        private readonly IServicioComandos comandos;
        private readonly IServicioOrquestador orquestador;

        public ServicioLlamadoAutomatico(IServicioRepositorio repositorio, ILogger log, IServicioComandos comandos, IServicioOrquestador orquestador)
        {
            this.repositorio = repositorio;
            this.log = log;
            this.comandos = comandos;
            this.orquestador = orquestador;
        }

        public void Llamar(LlamadoAutomatico tipoLlamadoAutomatico)
        {
            switch (tipoLlamadoAutomatico)
            {
                case LlamadoAutomatico.Granos:
                    LlamarAutomaticoGranos();
                    break;

                case LlamadoAutomatico.NoGranos:
                    LlamarAutomaticoNoGranos();
                    break;
            }
        }

        public void Detener(LlamadoAutomatico tipoLlamadoAutomatico)
        {
            switch (tipoLlamadoAutomatico)
            {
                case LlamadoAutomatico.Granos:
                    DetenerLlamadoAutomaticoGranos();
                    break;
            }
        }

        private void LlamarAutomaticoGranos()
        {
            var configuracionGeneral = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);
            if (configuracionGeneral == null
                || string.IsNullOrEmpty(configuracionGeneral.Valor)
                || !bool.TryParse(configuracionGeneral.Valor, out bool automatismoGranoGeneral)
                || !automatismoGranoGeneral)
                return;

            if (!repositorio.ExisteEspacioDisponibleParaLlamarEnCartel(CodigoMensajeCartelLed.LlamadoCallePreBalanza))
                return;

            var callesPreHidraulica = repositorio.ListarCallesPorTipo(TipoCalle.PlayaInterna).Where(x => !x.Deshabilitada);
            foreach (var calle in callesPreHidraulica)
                ValidarTipoLlamadoAutomaticoGrano(calle);
        }

        private void ValidarTipoLlamadoAutomaticoGrano(CalleDto callePH)
        {
            if (callePH.AutomatismoTipoLlamado.Codigo == Constantes.AutomatismoTipoLlamado.PaseDirecto)
                ValidarLlamadoPorPasoDirecto(callePH);
            else if (callePH.AutomatismoTipoLlamado.Codigo == Constantes.AutomatismoTipoLlamado.UnoAUno)
                ValidarLlamadoPor1A1(callePH);
            else if (callePH.AutomatismoTipoLlamado.Codigo == Constantes.AutomatismoTipoLlamado.PorFila)
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
            var camionesEnCallesPB = new List<CallePorRecorridoDto>();
            var configuraciones = repositorio.ListarAutomatismoGrano().Where(x => x.Activo && x.CallePreHidraulicaId == callePH.Id);
            foreach (var configuracion in configuraciones)
            {
                var camiones = repositorio.ListarCallePorRecorridoPorCalleId(configuracion.CallePreBalanzaId);
                camionesEnCallesPB.AddRange(camiones);
            }

            if (repositorio.ExisteLlamadoCallePreBalanzaPorTipoDeLlamado(callePH.Id, callePH.AutomatismoTipoLlamado.Codigo) && !camionesEnCallesPB.Any())
                return;

            if (!repositorio.ValidarEspacioDisponibleEnCallePreHidraulica(callePH.Id))
                return;
            var camionLlamado = camionesEnCallesPB.OrderBy(x => x.FechaIngeso).FirstOrDefault();
            if (camionLlamado != null)
                LlamarCamionPreBalanza(callePH.Id, camionLlamado, esCamionEnEspera: false);
            var camionEnEspera = camionesEnCallesPB.OrderBy(x => x.FechaIngeso).Skip(1).FirstOrDefault();
            if (camionEnEspera != null)
                LlamarCamionPreBalanza(callePH.Id, camionEnEspera, esCamionEnEspera: true);
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
                CalleId = callePHId,
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
            var camionEnEspera = repositorio.ObtenerCallePreBalanzaPlayaInternaDeCamionEnEspera(callePreBalanzaPlayaInterna.CallePlayaInterna.Id);
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
                CalleId = callePreBalanzaPlayaInterna.CallePlayaInterna.Id,
                LimpiarCamion = true,
                RecorridoId = callePreBalanzaPlayaInterna.RecorridoId,
            }) as ResultadoMensajeCartelLedReordenado;

            if (!resultadoLimpiarCalleCartelLed.HayErrores && resultadoLimpiarCalleCartelLed.ListaDeMensajes.Any())
                EnviarMensajesAlCartel(resultadoLimpiarCalleCartelLed.ListaDeMensajes);
        }

        private void LlamarAutomaticoNoGranos()
        {
            var recorridos = repositorio.ObtenerPrimerosRecorridosDisponibleParaLlamadoAutomaticoNoGranos();

            log.Info("fer-nogranos01-cantidadRecorridos" + recorridos.Count());

            foreach (var recorrido in recorridos)
            {
                
                var cartelLed = repositorio.ObtenerCartelDisponible(CodigoMensajeCartelLed.LlamadoCamionNoGrano);

                log.Info("fer-nogranos02-cartelDisponible:" + (cartelLed == null).ToString());

                if (cartelLed == null)
                {
                    break;                    
                }

                var lugaresDisponibles = repositorio.ObtenerDisponibilidadEnPlayaInternaNoGranos(recorrido.CalleId.Value);

                log.Info("fer-nogranos03-lugaresDisponibles:" + lugaresDisponibles);
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

                    log.Info("fer-nogranos04 llamo a camion:" + recorrido.Id);
                }
            }
        }
    }
}