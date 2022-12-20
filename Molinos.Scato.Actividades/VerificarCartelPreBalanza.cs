using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.Scato.Actividades
{
    public class VerificarCartelPreBalanza : CodeActivity<Resultado>
    {
        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new ResultadoMensajeCartelLedReordenado();
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            try
            {
                var callePlayaInternaList = repositorio.ListarCallesPorTipo(TipoCalle.PlayaInterna).Where(q => !q.Deshabilitada);

                foreach (var callePlayaInterna in callePlayaInternaList)
                {
                    var callePreBalanzaList = repositorio.ListarCallesPreBalanzaPorCallePlayaInternaId(callePlayaInterna.Id);
                    foreach (var callePreBalanza in callePreBalanzaList)
                    {
                        var camionesEnFilaPreBalanza = repositorio.ListarCallePorRecorridoPorCalleId(callePreBalanza.Id);
                        if (!camionesEnFilaPreBalanza.Any())
                        {
                            servicio.Ejecutar(new EliminarCallePreBalanzaPlayaInterna()
                            {
                                CallePlayaInternaId = callePlayaInterna.Id,
                                CallePreBalanzaId = callePreBalanza.Id
                            });
                            resultado = servicio.Ejecutar(new ModificarHistorialMensajeCartelLed()
                            {
                                Codigo = CodigoMensajeCartelLed.CartelPreBalanza,
                                CalleId = callePreBalanza.Id
                            }) as ResultadoMensajeCartelLedReordenado;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado.Error("ErrorException", ex.Message);
            }

            if(!resultado.HayErrores && resultado.ListaDeMensajes.Any())
            {
                var cartel = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoPlayaInterna, Constantes.ConfiguracionGeneral.PreBalanza.CartelLedPreBalanza);
                foreach (var mensajeCartelLed in resultado.ListaDeMensajes)
                {
                    servicio.Ejecutar(new EnviarMensajeCartelLed
                    {
                        Mensaje = mensajeCartelLed.HistorialMensajeCartelLed?.Mensaje ?? string.Empty,
                        Codigo = cartel?.Valor,
                        NumeroTrama = mensajeCartelLed.Trama,
                        NumeroPrograma = mensajeCartelLed.Programa,
                        NumeroVariable = mensajeCartelLed.Variable,
                    });
                }
            }

            return resultado;
        }
    }
}