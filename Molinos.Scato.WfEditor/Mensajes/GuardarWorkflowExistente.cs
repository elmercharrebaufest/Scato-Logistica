using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.WfEditor.Mensajes
{
    public class GuardarWorkflowExistente
    {
        public int WorkflowId { get; set; }
        public WorkflowDefinicionDto Definicion { get; set; }
    }
}
