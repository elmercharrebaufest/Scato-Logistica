using System;
using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarCorrespondeCaladoEnPlanta : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        public OutArgument<bool> Corresponde { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var correspondeDescarga = srvRepositorio.VerificarCorrespondeCaladoEnPlanta(instanceId);

            Corresponde.Set(context, correspondeDescarga);
        }
    }
}