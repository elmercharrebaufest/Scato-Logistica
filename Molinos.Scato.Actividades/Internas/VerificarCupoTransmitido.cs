using Molinos.Scato.Servicios;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarCupoTransmitido : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Cupo { get; set; }
        public OutArgument<bool> Corresponde { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();

            var instanceId = context.WorkflowInstanceId;
            var cartaporte = repositorio.ObtenerCartaPortePorInstanceId(instanceId);
            var cupo = Cupo.Get<string>(context);
            Corresponde.Set(context, !repositorio.CupoTransmitido(cupo, cartaporte.NroCartaPorte));
        }
    }
}
