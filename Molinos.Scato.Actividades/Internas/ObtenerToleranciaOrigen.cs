using System;
using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ObtenerToleranciaOrigen : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        public OutArgument<int> Tolerancia { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var servicio = context.GetExtension<IServicioRepositorio>();

                Tolerancia.Set(context, servicio.ObtenerToleranciaOrigen(InstanceId.Get<Guid>(context)));
            }
            catch
            {
                
            }
        }
    }
}
