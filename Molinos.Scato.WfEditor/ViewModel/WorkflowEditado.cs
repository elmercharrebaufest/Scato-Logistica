using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.WfEditor.ViewModel
{
    public class WorkflowEditado
    {
        public WorkflowDefinicionDto Definicion { get; set; }
        public byte[] Xamlx { get; set; }
    }
}
