using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarKilosDeclaradosPorFinca : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> VinedoId { get; set; }

        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }

        [RequiredArgument]
        public InArgument<string> Cosecha { get; set; }

        [RequiredArgument]
        public OutArgument<bool> ExcedeKilosARecibir { get; set; }

        [RequiredArgument]
        public OutArgument<bool> AvisoDeCorte { get; set; }

        [RequiredArgument]
        public OutArgument<decimal> KilosARecibir { get; set; }

        [RequiredArgument]
        public OutArgument<string> Vinedo { get; set; }

        [RequiredArgument]
        public OutArgument<string> Variedad { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioRepositorio>();

            var resultado = servicio.VerificarKilosDeclaradosPorVinedo(VinedoId.Get(context), MaterialId.Get(context), Cosecha.Get(context));

            ExcedeKilosARecibir.Set(context, resultado.ExcedeKilosARecibir || resultado.Error);
            AvisoDeCorte.Set(context, resultado.AvisoDeCorte);
            KilosARecibir.Set(context, resultado.KilosARecibir);
            Vinedo.Set(context, resultado.Vinedo);
            Variedad.Set(context, resultado.Variedad);
        }
    }
}