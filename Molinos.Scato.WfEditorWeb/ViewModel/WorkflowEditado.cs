using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.WfEditorWeb.ViewModel
{
    public class WorkflowEditado
    {
        public WorkflowDefinicionDto Definicion { get; set; }
        public byte[] Xamlx { get; set; }
    }
}
