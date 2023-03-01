using System;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class AutorizarCpeDG : Comando
    {
        public Guid WorkflowId { get; set; }
        public TipoDocumentoIngreso TipoDocumento { get; set; }
        public int NroOrden { get; set; }
        public int Sucursal { get; set; }
        public short TipoCPE { get; set; }
        public int PuestoDeTrabajoId { get; set; }
    }
}
