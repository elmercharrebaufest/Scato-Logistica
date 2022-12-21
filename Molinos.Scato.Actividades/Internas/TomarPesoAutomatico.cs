using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Linq;

namespace Molinos.Scato.Actividades.Internas
{

    public class TomarPesoAutomatico : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> Recorrido { get; set; }
        public OutArgument<Resultado> Error { get; set; }
        public OutArgument<int> Peso { get; set; }
        public OutArgument<int> BalanzaId { get; set; }
        public InArgument<TipoPesada> TipoPesada { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var servComando = context.GetExtension<IServicioComandos>();
            var servicio = context.GetExtension<IServicioNotificarUsuario>();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            var recorrido = Recorrido.Get<ControlRecorridoDto>(context);

            var pesaje = new ResultadoPesaje();
            Error.Set(context, new Resultado());
            var tipoPesada = TipoPesada.Get<TipoPesada>(context);
            if ((recorrido.ActividadXaml == "PesadaBruto" && tipoPesada == Dominio.Enums.TipoPesada.Bruto) ||
                (recorrido.ActividadXaml == "PesadaTara" && tipoPesada == Dominio.Enums.TipoPesada.Tara))
            {

                try
                {
                
                    pesaje = (ResultadoPesaje)servComando.Ejecutar(new ObtenerPesada() { Recorrido = recorrido });
                    if (pesaje.HayErrores)
                    {
                        recorrido.Comentario = pesaje.Errores.First().Value;
                        servComando.Ejecutar(new CrearControlRecorrido { Dto = recorrido });
                        Error.Set(context, pesaje);
                    }
                    else
                    {
                        if(recorrido.ActividadXaml == "PesadaBruto" && tipoPesada == Dominio.Enums.TipoPesada.Bruto)
                        {
                            ContigenciaDePesosExcedidos(repositorio, servComando, pesaje, recorrido);
                        }
                        Peso.Set(context, pesaje.Peso);
                        BalanzaId.Set(context, pesaje.BalanzaId);
                    }
                
                }
                catch (Exception ex)
                {
                    pesaje.Error("", ex.Message);
                    Error.Set(context, pesaje);
                }
                
                try
                {
                    if(pesaje.Peso < 1000)
                    {
                        pesaje.Error("", "Peso en balanza por debajo del mínimo, por favor revisar balanza");
                        Error.Set(context, pesaje);
                    }
                    var notificacion = new NotificacionPesadaAutomaticaDto
                    {
                        Id = recorrido.PuestoDeTrabajoId,
                        Error = pesaje.HayErrores ? pesaje.Errores.First().Value : null,
                        CartaPorte = recorrido.CartaDePorte,
                        Diferencia = ((recorrido.PesoBruto != null || recorrido.PesoTara != null) ?(
                        recorrido.PesoBruto != null ? recorrido.PesoBruto - pesaje.Peso :
                                                      pesaje.Peso - recorrido.PesoTara)
                        : 0).ToString(),
                        Peso = pesaje.Peso.ToString(),
                        Entregador = recorrido.Entregador ? "Si" : "No",
                        Material = recorrido.Material,
                        WorkflowInstanceId = recorrido.WorkflowInstanceId,
                        TipoPeso = recorrido.Actividad.Contains("Bruto") ? "Bruto:" :
                     recorrido.Actividad.Contains("Tara") ? "Tara:" : "",
                        Patente = recorrido.Patente,
                        Actividad = recorrido.Actividad,
                        DifPeso = (recorrido.Actividad.Contains("Bruto") ? pesaje.Peso - (recorrido.PesoOrigenBruto ?? 0) :
                     recorrido.Actividad.Contains("Tara") ? pesaje.Peso - (recorrido.PesoOrigenTara ?? 0) : 0).ToString(),
                        TipoVehiculo = recorrido.TipoVehiculo.ToString(),
                        Tarjeta = recorrido.Tarjeta,

                        DifNeto = (recorrido.PesoOrigenNeto.HasValue && (recorrido.PesoBruto != null || recorrido.PesoTara != null) ? (
                        recorrido.PesoBruto != null ? recorrido.PesoBruto - pesaje.Peso - recorrido.PesoOrigenNeto :
                                                      pesaje.Peso - recorrido.PesoTara- recorrido.PesoOrigenNeto)
                        : 0).ToString(),
                        Calle = recorrido.Calle,
                        TipoPesoOrigen = recorrido.Actividad.Contains("Bruto") ? "Bruto Org:" :
                     recorrido.Actividad.Contains("Tara") ? "Tara Org:" : "",
                        PesoBrutoOrigen = recorrido.PesoOrigenBruto.ToString(),
                        PesoNetoOrigen = recorrido.PesoOrigenNeto.ToString()
                    
                    };
                    
                
                    servicio.Notificar(new NotificacionDto
                    {
                        Grupo = "Automaticas",
                        Mensaje = notificacion.ToJson(),
                        TipoAlerta = TipoAlerta.Automatica,
                        Leido = !pesaje.HayErrores,
                        PuestoId = recorrido.PuestoDeTrabajoId
                    });
                }
                catch(Exception e)
                {
                    recorrido.Comentario = e.Message;
                    servComando.Ejecutar(new CrearControlRecorrido { Dto = recorrido });
                }
                try
                {
                    servComando.Ejecutar(new ModificarBalanzaEstaEnCero
                    {
                        BalanzaId = pesaje.BalanzaId,
                        EstaEnCero = false
                    });
                }
                catch (Exception e)
                {
                    recorrido.Comentario = e.Message;
                    servComando.Ejecutar(new CrearControlRecorrido { Dto = recorrido });
                }

            }
            else
            {
                var mensaje = "La etapa a ejecutar no coincide con el estado del workflow.";
                pesaje.Error("2", mensaje);
                Error.Set(context, pesaje);
            }
        }

        private void ContigenciaDePesosExcedidos(IServicioRepositorio repositorio, IServicioComandos servicioComando, ResultadoPesaje pesaje, ControlRecorridoDto recorrido)
        {
            var balanza = repositorio.ObtenerBalanza(pesaje.BalanzaId);
            if (balanza.ContingenciaExcedentesHabilitada && balanza.ToleranciaExcedida != null && recorrido.RecorridoId != null)
            {
                var pesoMaximoPorTipoVehiculo = repositorio.LeerPesoMaximo(recorrido.WorkflowInstanceId, recorrido.TipoDeWorkflow);
                if (pesaje.Peso > pesoMaximoPorTipoVehiculo && (pesaje.Peso - pesoMaximoPorTipoVehiculo <= balanza.ToleranciaExcedida))
                {
                    servicioComando.Ejecutar(new CrearPesosExc()
                    {
                        RecorridoId = recorrido.RecorridoId.Value,
                        PesoTomado = pesaje.Peso
                    });
                    pesaje.Peso = pesoMaximoPorTipoVehiculo;
                }
            }
        }
    }
}
