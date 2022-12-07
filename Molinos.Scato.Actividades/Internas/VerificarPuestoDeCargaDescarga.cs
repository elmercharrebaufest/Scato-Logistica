using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Linq;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarPuestoDeCargaDescarga : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        public OutArgument<bool> PuestoCorrecto { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();

            try
            {
                var servicio = context.GetExtension<IServicioRepositorio>();
                var servicioComando = context.GetExtension<IServicioComandos>();
                var instanceId = InstanceId.Get(context);
                var puestoId = PuestoDeTrabajoId.Get(context);

                var puestoCorrecto = servicio.VerificarPuestodeCargaDescarga(instanceId, puestoId);
                PuestoCorrecto.Set(context, puestoCorrecto);
                if (!puestoCorrecto)
                {
                    resultado.Errores.Add("", Textos.ConfirmacionDeDescarga_PuestoIncorrecto);
                }
                else
                {
                    var recorrido = servicio.ObtenerRecorridoPorGuid(instanceId);
                    var puestoDeCargaDescarga = servicio.ObtenerPuestoDeCargaDescargaPorPuestoId(puestoId);
                    if(puestoDeCargaDescarga != null)
                    {
                        servicioComando.Ejecutar(new ActualizarLlamadoAutomaticoHidraulica
                        {
                            Id = puestoDeCargaDescarga.Id,
                            Estado = EstadoHidraulica.Ocupado,
                            Patente = recorrido.Patente
                        });

                        // Se liberan hidraulicas que llamaron incorrectamente a una misma patente, el cual ya se encuentra en otra hidraulica
                        var hidraulicasPorLiberar = servicio.ListarHidraulicasPorEstado(EstadoHidraulica.Llamando).Where(x => x.UltimaPatenteLlamada == recorrido.Patente);
                        foreach (var hidraulica in hidraulicasPorLiberar)
                        {
                            servicioComando.Ejecutar(new ActualizarLlamadoAutomaticoHidraulica
                            {
                                Id = hidraulica.HidraulicaId,
                                Estado = EstadoHidraulica.Disponible,
                                Patente = string.Empty
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("WorkflowId", e.Message);
            }
            return resultado;
        }
    }
}