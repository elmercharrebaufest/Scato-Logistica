using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.Scato.Actividades
{
    public class LlamarCallePreBalanza : CodeActivity<Resultado>
    {
        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var configuracionLlamadoAutomatico = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoPlayaInterna, Constantes.ConfiguracionGeneral.PreBalanza.LlamadoAutomatico);
            if (configuracionLlamadoAutomatico == null || !bool.TryParse(configuracionLlamadoAutomatico.Valor, out bool llamadoAutomaticoActivo) || !llamadoAutomaticoActivo)
                return resultado;

            var callePlayaInternaList = repositorio.ListarCallesPorTipo(TipoCalle.PlayaInterna).Where(q => !q.Deshabilitada);
            var callePreBalanzaList = repositorio.ListarCallesPorTipo(TipoCalle.PreBalanzaGranos).Where(q => !q.Deshabilitada && q.FechaLLamada.Equals(null));

            foreach (var callePlayaInterna in callePlayaInternaList)
            {
                if (ExisteSlotsDisponibles(repositorio, callePlayaInterna))
                {
                    var listaDePrimerosCamiones = new List<CallePorRecorridoDto>();
                    foreach (var callePreBalanza in callePreBalanzaList)
                    {
                        var camionesPorCallePreBalanza = repositorio.ListarCallePorRecorridoPorCalleId(callePreBalanza.Id)
                            .OrderBy(q => q.Id);

                        if (camionesPorCallePreBalanza.Count() < callePreBalanza.CantidadDeCamiones)
                            continue;

                        var camionMasAntiguo = camionesPorCallePreBalanza.FirstOrDefault();

                        if (camionMasAntiguo?.CalleRecorridoId == callePlayaInterna.Id)
                            listaDePrimerosCamiones.Add(camionMasAntiguo);
                    }

                    if (listaDePrimerosCamiones.Any())
                    {
                        var camionLlamado = listaDePrimerosCamiones.OrderBy(x => x.Id).FirstOrDefault();
                        LlamarCalle(servicio, repositorio, callePlayaInterna.Id, camionLlamado.CalleId);
                        break;
                    }
                }
            }

            return resultado;
        }

        private bool ExisteSlotsDisponibles(IServicioRepositorio repositorio, CalleDto callePlayaInterna)
        {
            var camionesEnPlayaInterna = repositorio.ListarCallePorRecorridoPorCalleId(callePlayaInterna.Id).Count();
            var camionesEnPreBalanza = repositorio.ObtenerCantidadCamionesLlamadosEnCallePreBalanza(callePlayaInterna.Id);

            var slotsLibres = callePlayaInterna.CantidadDeCamiones - (camionesEnPlayaInterna + camionesEnPreBalanza);
            var slotNecesario = int.Parse(ConfigurationManager.AppSettings["SlotNecesariosLlamadaPreBalanza"]);
            return slotsLibres >= slotNecesario;
        }

        private void LlamarCalle(IServicioComandos servicio, IServicioRepositorio repositorio, int callePlayaInternaId, int callePreBalanzaId)
        {
            servicio.Ejecutar(new CrearCallePreBalanzaPlayaInterna { CallePlayaInternaId = callePlayaInternaId, CallePreBalanzaId = callePreBalanzaId });
            var cartel = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoPlayaInterna, Constantes.ConfiguracionGeneral.PreBalanza.CartelLedPreBalanza);
            var resultadoInsertarCartelLed = servicio.Ejecutar(new InsertarSlotMensajeCartelLed()
            {
                Codigo = CodigoMensajeCartelLed.LlamadoCallePreBalanza,
                CalleId = callePreBalanzaId
            }) as ResultadoMensajeCartelLed;
            servicio.Ejecutar(new EnviarMensajeCartelLed
            {
                Mensaje = resultadoInsertarCartelLed.Mensaje,
                Codigo = cartel?.Valor,
                NumeroTrama = resultadoInsertarCartelLed.NumeroTrama,
                NumeroPrograma = resultadoInsertarCartelLed.NumeroPrograma,
                NumeroVariable = resultadoInsertarCartelLed.NumeroVariable,
            });
        }
    }
}