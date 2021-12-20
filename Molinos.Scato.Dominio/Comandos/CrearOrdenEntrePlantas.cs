using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearOrdenEntrePlantas : Comando
    {
        public OrdenEntrePlantasDto Orden { get; set; }
        public string NombreWorkflow { get; set; }
        public int WorkflowDefinicionId { get; set; }
        public Guid InstanciaWorkflowId { get; set; }
        public int CentroId { get; set; }
    }
}
