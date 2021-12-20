using System;
using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarEsPuestoAutomatico : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> PuestoId { get; set; }
        public OutArgument<bool> EsAutomatico { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var puestoId = PuestoId.Get<int>(context);
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var correspondeDescarga = srvRepositorio.EsPuestoFullAutomatizado(puestoId);

            EsAutomatico.Set(context, correspondeDescarga);
        }
    }
}