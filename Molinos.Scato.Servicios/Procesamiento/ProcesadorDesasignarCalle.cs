using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorDesasignarCalle : ProcesadorComando<DesasignarCalle>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicioRepositorio;

        public ProcesadorDesasignarCalle(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioRepositorio servicioRepositorio)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.servicioRepositorio = servicioRepositorio;
        }

        public override Resultado Ejecutar(DesasignarCalle comando)
        {
            var asignaciones = Repositorio.Listar<CallePorRecorrido>(
                x => x.FechaEgreso == null &&
                x.Id != comando.UltimaAsignacionId &&
                (x.Recorrido.InstanciaWorkflow == comando.InstanciaWorkflow || x.CargaDeCupo.Recorrido.InstanciaWorkflow == comando.InstanciaWorkflow));
            if (asignaciones.Any())
            {
                foreach (var asignacion in asignaciones)
                {
                    
                    asignacion.FechaEgreso = DateTime.Now;
                    if (asignacion.Calle.TipoCalle == TipoCalle.PreCalado ||
                        asignacion.Calle.TipoCalle == TipoCalle.Circular ||
                        asignacion.Calle.TipoCalle == TipoCalle.PostCalado)
                    {
                        LiberarFilaSiQuedaVacia(asignacion);
                    }
                }
                Repositorio.GuardarCambios();
            }

            return new Resultado();
        }

        private void LiberarFilaSiQuedaVacia(CallePorRecorrido asignacion)
        {
            var camionesEnFila = Repositorio.Contar<CallePorRecorrido>(x => x.FechaEgreso == null && x.Calle.Id == asignacion.Calle.Id);
            if (camionesEnFila == 1)
            {
                asignacion.Calle.Bloqueada = false;
                asignacion.Calle.FechaLLamada = null;

                if (asignacion.Calle.TipoCalle == TipoCalle.PreCalado ||
                    asignacion.Calle.TipoCalle == TipoCalle.Circular)
                {
                    var calleCaladoId = asignacion.Calle.CalleCalado?.Id;
                    var mensajeCartelLedCaladorEntity = Repositorio.ObtenerPrimero<MensajeCartelLedCalador>(x => x.Calle.Id == calleCaladoId);
                    if (mensajeCartelLedCaladorEntity != null)
                    {
                        var codigo = mensajeCartelLedCaladorEntity.MensajeCartelLed.Codigo;
                        var resultado = (ResultadoMensajeCartelLedReordenado)servicioComandos.Ejecutar(new LimpiarHistorialMensajeCartelLed
                        {
                            CalleId = asignacion.Calle.Id,
                            Codigo = codigo
                        });
                        asignacion.Calle.CalleCalado = null;

                        var cartel = servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePreCalado, Constantes.ConfiguracionGeneral.PreCalado.CartelLedCalador);
                        LimpiarHistorialMensajeCartelLed(cartel?.Valor, resultado.ListaDeMensajes);
                    }
                }

                if (asignacion.Calle.TipoCalle == TipoCalle.PostCalado)
                {
                    
                    var resultado = (ResultadoMensajeCartelLedReordenado)servicioComandos.Ejecutar(new LimpiarHistorialMensajeCartelLed
                    {
                        CalleId = asignacion.Calle.Id,
                        Codigo = CodigoMensajeCartelLed.LlamadoCallePostcalado
                    });

                    var cartel = servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePostCalado, Constantes.ConfiguracionGeneral.PostCalado.CartelLedPostCalado);
                    LimpiarHistorialMensajeCartelLed(cartel?.Valor, resultado.ListaDeMensajes);

                    Log.Debug("se realizo la desasignacion de calle postcalado, recorridoId:" + asignacion?.Recorrido?.Id + ", calleId: " + asignacion?.Calle?.Id);
                }
            }
        }

        private void LimpiarHistorialMensajeCartelLed(string codigoCartel, List<MensajeCartelLedDto> listaDeMensajes)
        {
            foreach (var mensajeCartelLed in listaDeMensajes)
            {
                servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                {
                    Mensaje = mensajeCartelLed.HistorialMensajeCartelLed?.Mensaje ?? "-",
                    Codigo = codigoCartel,
                    NumeroTrama = mensajeCartelLed.Trama,
                    NumeroPrograma = mensajeCartelLed.Programa,
                    NumeroVariable = mensajeCartelLed.Variable,
                });
            }
        }
    }
}