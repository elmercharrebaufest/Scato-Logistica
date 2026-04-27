using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class AutorizarCpe : Comando
    {
        
        public int CentroId { get; set; }
        public CartaPorteDto Dto { get; set; }
        public int NroOrden { get; set; }
        public int Sucursal { get; set; }
        public short TipoCPE { get; set; }
        public Guid WorkflowId { get; set; }
        public VehiculoDto Vehiculo { get; set; }
    }
}
