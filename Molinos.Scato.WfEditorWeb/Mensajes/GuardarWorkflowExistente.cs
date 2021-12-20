using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.WfEditorWeb.Mensajes
{
    public class GuardarWorkflowExistente
    {
        public int WorkflowId { get; set; }
        public WorkflowDefinicionDto Definicion { get; set; }
    }
}
