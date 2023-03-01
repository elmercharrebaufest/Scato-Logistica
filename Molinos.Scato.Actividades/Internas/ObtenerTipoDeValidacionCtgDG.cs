using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public sealed class ObtenerTipoDeValidacionCtgDG : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public OutArgument<bool> UsarValidacionAutomatica { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var centroId = CentroId.Get<int>(context);
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();

            UsarValidacionAutomatica.Set(context, srvRepositorio.ValidacionAutomaticaCtgDG(centroId));
        }
    }
}