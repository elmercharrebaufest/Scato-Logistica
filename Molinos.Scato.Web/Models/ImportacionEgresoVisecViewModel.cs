using System;

namespace Molinos.Scato.Web.Models
{
    public class ImportacionEgresoVisecViewModel
    {
        public string CartaPorte { get; set; }
        public string Patente { get; set; }
        public string Material { get; set; }
        public int PesoVisecRequerido { get; set; }
        public Guid InstanciaWorkflow { get; set; }
        public int WorkflowDefinicionId { get; set; }
    }
}