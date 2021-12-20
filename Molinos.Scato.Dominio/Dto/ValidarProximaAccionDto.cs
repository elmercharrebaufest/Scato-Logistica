using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ValidarProximaAccionDto
    {
        public string ProximaActividad { get; set; }
        public string MensajeError { get; set; }
        public bool Valida { get; set; }
        public Guid InstanceId { get; set; }
        public int PuestoDeTrabajoId { get; set; }
        public int WorkflowDefinicionId { get; set; }
        public string CodigoSapCentro { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public string Patente { get; set; }
    }
}
