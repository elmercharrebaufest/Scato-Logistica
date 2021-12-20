using System;
using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ObtenerToleranciaRomaneo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
            
        public OutArgument<int> Tolerancia { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var servicio = context.GetExtension<IServicioRepositorio>();

                Tolerancia.Set(context, servicio.ObtenerToleranciaRomaneo(InstanceId.Get<Guid>(context)));
            }
            catch
            {
                
            }
            
        }
    }
}
