using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class BajaCTGRetransmisionDto
    {
        public int CentroId { get; set; }
        public Guid WorkflowId { get; set; }
        public CartaPorteDto Dto { get; set; }
        public VehiculoDto Vehiculo { get; set; }
        public EstadoTransmisionASap EstadoCtg { get; set; }
        public EstadoTransmisionASap EstadoCtgDefinitivo { get; set; }
        public OrdenDeDescargaFasonDto OrdenDeDescargaFason { get; set; }
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
    }
}