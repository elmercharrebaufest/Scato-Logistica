using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
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
            log.Debug("Automatismo General: Activo");

            var configuraciones = repositorio.ListarAutomatismoGrano().Where(x => x.Activo);
            foreach (var configuracion in configuraciones)
                ValidarTipoLlamadoAutomaticoGrano(configuracion);
        }

        private void ValidarTipoLlamadoAutomaticoGrano(AutomatismoGranoDto configuracion)
        {
            if (!repositorio.ExisteCamionesEnCalle(configuracion.CallePreBalanzaId))
                return;

            var callePrebalanza = repositorio.ObtenerCalle(configuracion.CallePreBalanzaId);
            if (callePrebalanza.Bloqueada && callePrebalanza.FechaLLamada != null)
                return;

            log.Debug("Llamando Automatismo Por Configuracion Id: " + configuracion.Id);
            if (configuracion.EsPasoDirecto)
                ValidarLlamadoPasoDirecto(configuracion);
            else if (configuracion.Llamado1a1)
                ValidarLlamar1A1(configuracion);
            else
                ValidarLlamadoPorFila(configuracion);
        }

        private void ValidarLlamadoPasoDirecto(AutomatismoGranoDto configuracion)
        {
            if (ValidarEspacioDisponible(configuracion))
                LlamarCallePreBalanza(configuracion);
        }

        private void ValidarLlamadoPorFila(AutomatismoGranoDto configuracion)
        {
            var estaVacia = !repositorio.ListarCallePorRecorridoPorCalleId(configuracion.CallePreHidraulicaId).Any();
            if (estaVacia)
                LlamarCallePreBalanza(configuracion);
        }

        private void ValidarLlamar1A1(AutomatismoGranoDto configuracion)
        {
            if (ValidarEspacioDisponible(configuracion))
                Llamar1A1(configuracion);
        }

        private bool ValidarEspacioDisponible(AutomatismoGranoDto configuracion)
        {
            var callePreHidraulica = repositorio.ObtenerCalle(configuracion.CallePreHidraulicaId);
            return callePreHidraulica.CantidadDeCamiones > repositorio.ListarCallePorRecorridoPorCalleId(configuracion.CallePreHidraulicaId).Count();
        }

        private void Llamar1A1(AutomatismoGranoDto configuracion)
        {
            var camionesEnPrebalanza = repositorio
                                        .ListarCallePorRecorridoPorCalleId(configuracion.CallePreBalanzaId)
                                        .OrderBy(x => x.FechaIngeso);
            var camionLlamado = camionesEnPrebalanza.FirstOrDefault();
            if (camionLlamado != null)
                LlamarCamionPreBalanza(camionLlamado, esCamionEnEspera: false);

            var camionEnEspera = camionesEnPrebalanza.Skip(1).FirstOrDefault();
            if (camionEnEspera != null)
                LlamarCamionPreBalanza(camionEnEspera, esCamionEnEspera: true);
        }

        private void LlamarCamionPreBalanza(CallePorRecorridoDto camion, bool esCamionEnEspera)
        {
            var resultadoInsertarCalleCartelLed = comandos.Ejecutar(new InsertarSlotMensajeCartelLed()
            {
                Codigo = esCamionEnEspera ? CodigoMensajeCartelLed.LlamadoCamionPreBalanza : CodigoMensajeCartelLed.LlamadoCallePreBalanza,
                CalleId = camion.CalleId,
                EsLlamadoPorCamion = true,
                Patente = camion.Patente,
                EsCamionEnEspera = esCamionEnEspera,
                RecorridoId = camion.RecorridoId,
            }) as ResultadoMensajeCartelLed;
            if (!resultadoInsertarCalleCartelLed.HayErrores && resultadoInsertarCalleCartelLed.ListaDeMensajes.Any())
                EnviarMensajesAlCartel(resultadoInsertarCalleCartelLed.ListaDeMensajes);
        }

        private void LlamarCallePreBalanza(AutomatismoGranoDto configuracion)
        {
            var callePreBalanza = repositorio.ObtenerCalle(configuracion.CallePreBalanzaId);
            var resultado = ActualizarCalleLlamada(callePreBalanza);
            if (resultado.HayErrores)
                return;

            var resultadoInsertarCalleCartelLed = comandos.Ejecutar(new InsertarSlotMensajeCartelLed()
            {
                Codigo = CodigoMensajeCartelLed.LlamadoCallePreBalanza,
                CalleId = configuracion.CallePreBalanzaId,
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

        private Resultado ActualizarCalleLlamada(CalleDto calle)
        {
            calle.FechaLLamada = DateTime.Now;
            calle.Bloqueada = true;
            return comandos.Ejecutar(new ModificarCalleLlamada { Dto = calle });
        }

        private void DetenerLlamadoAutomaticoGranos()
        {
            var configuraciones = repositorio.ListarAutomatismoGrano();
            foreach (var configuracion in configuraciones)
                ValidarTipoLiberarAutomaticoGrano(configuracion);
        }

        private void ValidarTipoLiberarAutomaticoGrano(AutomatismoGranoDto configuracion)
        {
            if (configuracion.EsPasoDirecto)
                ValidarLiberarPorFila(configuracion);
            else if (configuracion.Llamado1a1)
                ValidarLiberarLlamado1a1(configuracion);
            else
                ValidarLiberarPorFila(configuracion);
        }

        private Resultado ActualizarCalleLiberada(CalleDto calle)
        {
            calle.FechaLLamada = null;
            calle.Bloqueada = false;
            return comandos.Ejecutar(new ModificarCalleLlamada { Dto = calle });
        }

        private void ValidarLiberarLlamado1a1(AutomatismoGranoDto configuracion)
        {
            ValidarLiberarLlamado1a1(configuracion, esCamionEnEspera: false);
            ValidarLiberarLlamado1a1(configuracion, esCamionEnEspera: true);
        }

        private void ValidarLiberarLlamado1a1(AutomatismoGranoDto configuracion, bool esCamionEnEspera)
        {
            var codigoMensajeCartel = esCamionEnEspera ? CodigoMensajeCartelLed.LlamadoCamionPreBalanza : CodigoMensajeCartelLed.LlamadoCallePreBalanza;
            var mensajeEnCartelLed = repositorio.ListarMensajesCartelLed(codigoMensajeCartel)
                                                    .Where(x => x.HistorialMensajeCartelLed.RecorridoId.HasValue
                                                            && x.HistorialMensajeCartelLed?.CalleId == configuracion.CallePreBalanzaId)
                                                    .Select(x => x.HistorialMensajeCartelLed)
                                                    .FirstOrDefault();

            if (mensajeEnCartelLed == null)
                return;

            var camiones = repositorio.ListarCallePorRecorridoPorCalleId(configuracion.CallePreBalanzaId);
            if (!camiones.Any())
            {
                LiberarLlamado1a1(mensajeEnCartelLed, codigoMensajeCartel, ultimoCamion: true);
                return;
            }
            
            if(esCamionEnEspera)
            {
                var segundoCamion = camiones.Skip(1).FirstOrDefault();
                if (segundoCamion == null)
                    LiberarLlamado1a1(mensajeEnCartelLed, codigoMensajeCartel, ultimoCamion: true);

                if (segundoCamion != null && segundoCamion.RecorridoId != mensajeEnCartelLed.RecorridoId)
                    LiberarLlamado1a1(mensajeEnCartelLed, codigoMensajeCartel);

            } else
            {
                var camion = repositorio.ObtenerCallePorRecorridoPorRecorridoIdYCalleId(mensajeEnCartelLed.RecorridoId.Value, configuracion.CallePreBalanzaId);
                if (camion.FechaEgreso.HasValue)
                    LiberarLlamado1a1(mensajeEnCartelLed, codigoMensajeCartel);
            }

        }

        private void LiberarLlamado1a1(HistorialMensajeCartelLedDto mensajeEnCartelLed, string codigoMensajeCartel, bool ultimoCamion = false)
        {
            var resultadoLimpiarCalleCartelLed = comandos.Ejecutar(new LimpiarHistorialMensajeCartelLed()
            {
                Codigo = codigoMensajeCartel,
                HistorialMensajeCartelLedId = mensajeEnCartelLed.Id,
                LimpiarCamion = true,
                UltimoCamion = ultimoCamion,
            }) as ResultadoMensajeCartelLedReordenado;
            if (!resultadoLimpiarCalleCartelLed.HayErrores && resultadoLimpiarCalleCartelLed.ListaDeMensajes.Any())
                EnviarMensajesAlCartel(resultadoLimpiarCalleCartelLed.ListaDeMensajes);
        }

        private void ValidarLiberarPorFila(AutomatismoGranoDto configuracion)
        {
            var callePreBalanza = repositorio.ObtenerCalle(configuracion.CallePreBalanzaId);
            if (!callePreBalanza.Bloqueada && callePreBalanza.FechaLLamada == null)
                return;

            var camionesEnCalle = repositorio.ListarCallePorRecorridoPorCalleId(configuracion.CallePreBalanzaId);
            if (camionesEnCalle.Any())
                return;

            LiberarCallePreBalanza(callePreBalanza);
        }

        private void LiberarCallePreBalanza(CalleDto calle)
        {
            var resultado = ActualizarCalleLiberada(calle);
            if (resultado.HayErrores)
                return;

            var resultadoLimpiarCalleCartelLed = comandos.Ejecutar(new LimpiarHistorialMensajeCartelLed()
            {
                Codigo = CodigoMensajeCartelLed.LlamadoCallePreBalanza,
                CalleId = calle.Id
            }) as ResultadoMensajeCartelLedReordenado;
            if (!resultadoLimpiarCalleCartelLed.HayErrores && resultadoLimpiarCalleCartelLed.ListaDeMensajes.Any())
                EnviarMensajesAlCartel(resultadoLimpiarCalleCartelLed.ListaDeMensajes);
        }

        private void LlamarAutomaticoNoGranos()
        {
            var cartelLed = repositorio.ObtenerCartelDisponible(CodigoMensajeCartelLed.LlamadoCamionNoGrano);

            if(cartelLed != null)
            {
                var recorrido = repositorio.ObtenerPrimerRecorridoDisponibleParaLlamadoAutomaticoNoGranos();

                if (recorrido == null) 
                    return;

                //validar que la patente de ese recorrido no este en el cartel?

                var lugaresDisponibles = repositorio.ObtenerDisponibilidadEnPlayaInternaNoGranos(recorrido.CalleId.Value);

                if (lugaresDisponibles > 0)
                {

                    var callePorRecorridoNoGranos = repositorio.ObtenerCallePorRecorridoPlayaExternaNoGranosPorRecorridoId(recorrido.Id);


                    var resultadoInsertarCalleCartelLed = comandos.Ejecutar(new InsertarSlotMensajeCartelLed()
                    {
                        Codigo = CodigoMensajeCartelLed.LlamadoCamionNoGrano,
                        CalleId = callePorRecorridoNoGranos.CalleId,
                        EsLlamadoPorCamion = true,
                        Patente = recorrido.Patente,
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

    }
}