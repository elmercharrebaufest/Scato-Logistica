using System.Activities;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ObtenerTipoComercial : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> TipoComercialId { get; set; }

        public OutArgument<TipoComercialDto> TipoComercial { get; set; }

        public OutArgument<int?> PesoEsperado { get; set; }

        public OutArgument<int> Tolerancia { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioRepositorio>();

            var tipoComercialId = TipoComercialId.Get<int>(context);

            var tipoComercial = servicio.ObtenerTipoComercial(tipoComercialId);
            var pesoEsperado = tipoComercial.PesoEsperado;
            var tolerancia = tipoComercial.ToleranciaDifPesoE != null ? tipoComercial.ToleranciaDifPesoE.Value : 0;

            TipoComercial.Set(context, tipoComercial);
            PesoEsperado.Set(context, pesoEsperado);
            Tolerancia.Set(context, tolerancia);
        }
    }
}
