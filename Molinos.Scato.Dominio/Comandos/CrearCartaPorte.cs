using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearCartaPorte : Comando
    {
        public CartaPorteDto Orden { get; set; }
        public VehiculoDto Vehiculo { get; set; }
        public string NombreWorkflow { get; set; }
        public int WorkflowDefinicionId { get; set; }
        public Guid InstanciaWorkflowId { get; set; }
        public int CentroId { get; set; }
    }
}
