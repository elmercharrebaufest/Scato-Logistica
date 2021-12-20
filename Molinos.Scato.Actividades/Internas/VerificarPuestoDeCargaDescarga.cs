using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

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
                var instanceId = InstanceId.Get(context);
                var puestoId = PuestoDeTrabajoId.Get(context);

                var puestoCorrecto = servicio.VerificarPuestodeCargaDescarga(instanceId, puestoId);
                PuestoCorrecto.Set(context, puestoCorrecto);
                if (!puestoCorrecto)
                {
                    resultado.Errores.Add("", Textos.ConfirmacionDeDescarga_PuestoIncorrecto);
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
