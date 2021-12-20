using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public sealed class ObtenerTipoDeValidacionCtg : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public OutArgument<bool> UsarValidacionAutomatica { get; set; }

        public OutArgument<bool> UsarValidacionManual { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var centroId = CentroId.Get<int>(context);
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();

            UsarValidacionAutomatica.Set(context, srvRepositorio.ValidacionCtgEsAutomatica(centroId));
            UsarValidacionManual.Set(context, srvRepositorio.ValidacionCtgEsManual(centroId));
        }
    }
}