using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DatosDeInstanciaDto
    {
        public int Id { get; set; }
        public int WorkflowDefinicionId { get; set; }
        public string WorkflowCodigo { get; set; }
        public int WorkflowId { get; set; }

        public string DatosProximaActividad { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
        public string TipoVariedadCodigo { get; set; }
    }
}