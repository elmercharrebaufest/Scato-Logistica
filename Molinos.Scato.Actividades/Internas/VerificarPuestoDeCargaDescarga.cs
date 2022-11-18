using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

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

                    servicioComando.Ejecutar(new ActualizarLlamadoAutomaticoHidraulica
                    {
                        Id = puestoDeCargaDescarga.Id,
                        Estado = EstadoHidraulica.Ocupado,
                        Patente = recorrido.Patente
                    });
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