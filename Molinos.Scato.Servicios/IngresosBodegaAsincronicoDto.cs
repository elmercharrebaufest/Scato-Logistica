using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios
{
    public sealed class IngresosBodegaAsincronicoDto
    {
        public int? TipoBinId { get; set; }
        public IngresosBodegaRequest IngresosBodega { get; set; }

    }
}