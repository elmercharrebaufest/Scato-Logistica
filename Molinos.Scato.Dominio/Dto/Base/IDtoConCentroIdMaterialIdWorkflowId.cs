using System;

namespace Molinos.Scato.Dominio.Dto
{
    public interface IDtoConCentroIdMaterialIdWorkflowId
    {
        int MaterialId { get; set; }
        int CentroId { get; set; }
        Guid WorkflowId { get; set; }
    }
}
