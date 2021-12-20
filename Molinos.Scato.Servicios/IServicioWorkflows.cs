using System.Collections.Generic;
using System.ServiceModel;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Servicios
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioWorkflows
    {
        [OperationContract]
        IList<WorkflowDto> ListarWorkflows();

        [OperationContract]
        IList<WorkflowDto> ListarWorkflowsPorCentro(int centroId);

        [OperationContract]
        IList<WorkflowDefinicionDto> ListarDefinicionesWorkflow(int idWorkflow);

        [OperationContract]
        WorkflowDefinicionDto ObtenerUltimaDefinicionWorkflow(int idWorkflow);

        [OperationContract]
        byte[] ObtenerArchivoDefinicionWorkflow(int idDefinicion);

        [OperationContract]
        void ActualizarDefinicion(int idDefinicion, byte[] xamlx, string actividadInicial, bool activar);

        [OperationContract]
        WorkflowDefinicionDto CrearWorkflow(WorkflowDefinicionDto definicion, byte[] xamlx);

        [OperationContract]
        WorkflowDefinicionDto CrearDefinicionWorkflow(int idWorkflow, WorkflowDefinicionDto definicion, byte[] xamlx);

        [OperationContract]
        void ActivarWorkflow(int idWorkflow, bool activar);

        [OperationContract]
        IList<CentroDto> ListarCentros(string nombreUsuario);

        [OperationContract]
        int ObtenerCentroIdPorWorkflowId(int idWorkflow);

        [OperationContract]
        List<string> ListarActividadesPorDefinicionWorkflow(int idWorkflow);

        [OperationContract]
        bool TienePermisoEditordeWorkflow(string nombreUsuario);
    }
}
