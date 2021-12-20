using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MuestraPuestoComandoDto
    {
        public int CaladoId { get; set; }
        public bool EsHumedad { get; set; }
        public bool EsGranosVerdes { get; set; }
        public bool EsGranosDañados { get; set; }
        public bool EsCuerposExtranos { get; set; }
    }
}
