using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class DarDeAltaCTG : Comando
    {
        public CartaPorteDto Dto { get; set; }
        public VehiculoDto Vehiculo { get; set; }
        public int CentroId { get; set; }
        public Guid WorkflowId { get; set; }
    }
}
