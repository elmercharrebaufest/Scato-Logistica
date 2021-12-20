using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{

    public class LiberarLimiteDeCreditoVenta : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid?> InstanceId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            
            var instanceId = InstanceId.Get<Guid>(context);

            var resultado = context.GetExtension<IServicioComandos>().Ejecutar(new Dominio.Comandos.LiberarStock{InstanceId = instanceId});
            
            return resultado;
        }
    }
}
