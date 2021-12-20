using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    /// <summary>
    ///     Retorna True solo si el chofer existe y tanto el como el camion estan habilitados
    /// </summary>
    public class VerificarTransportistaHabilitado : CodeActivity<bool>
    {
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        public InArgument<string> PatenteAcoplado { get; set; }

        [RequiredArgument]
        public InArgument<int> ChoferId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }


        protected override bool Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();

            var patente = Patente.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var choferId = ChoferId.Get<int>(context);
            var centroId = CentroId.Get<int>(context);

            return !repositorio.ChoferInhabilitado(choferId, centroId) && !repositorio.CamionInhabilitado(patente, centroId) && !repositorio.CamionInhabilitado(patenteAcoplado ?? "", centroId);
        }
    }
}