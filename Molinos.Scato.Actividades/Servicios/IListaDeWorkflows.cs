using System;
using System.Collections.Generic;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Servicios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IListaDeWorkflows" in both code and config file together.
    [ServiceContract]
    public interface IListaDeWorkflows
    {
        [OperationContract]
        Resultado EliminarInstanciaWorkflow(Guid instanciaId);
        [OperationContract]
        ListarWorkFlowsDto ListarWorkFlows(Paginacion paginacion, FiltroListaDeWorkflowsDto filtro);
        [OperationContract]
        ListarWorkFlowsDto ListarTotalWorkFlows(Paginacion paginacion, FiltroListaDeWorkflowsDto filtro);
        [OperationContract]
        InstanciaWorkflowDto ObtenerWorkflow(Guid instance);
        [OperationContract]
        IEnumerable<InstanciaWorkflowDto> ObtenerTotalWorkflows();
        [OperationContract]
        IEnumerable<InstanciaWorkflowDto> ObtenerWorkflowFiltro(int status, int condition);
        [OperationContract]
        InstanciaWorkflowDto ObtenerWorkflowPorPatente(string patente);
        [OperationContract]
        List<string> ObtenerWorkflowProximasAcciones(string nombreUsuario, int centroId);
        [OperationContract]
        ProximaAccionDto ObtenerWorkflowProximaAccion(Guid instancia);
        [OperationContract]
        bool VerificarExistenciaDeWorkflow(string patente);
        [OperationContract]
        bool VerificarExistenciaDeWorkflowPorGuid(Guid instancia);
        [OperationContract]
        InstanciaWorkflowDto ObtenerWorkflowPorGuid(Guid instancia);
        [OperationContract]
        Resultado ResumirInstanciaWorkflow(Guid instanciaId);
        [OperationContract]
        ProximaAccionEjecutableDto ObtenerWorkflowProximaAccionEjecutable(Guid instanciaId, string nombreUsuario, int centroId);
        [OperationContract]
        List<ServicioDto> ListarEstadoDeServicios();
        [OperationContract]
        List<GraficoDePlantaDto> ListarGraficoDePlanta(int centroId);
        [OperationContract]
        IList<InstanciaWorkflowPuertoDto> ListarEmbarques(string filtroProximaAccion = null);
        [OperationContract]
        List<InstanciaWorkflowDto> ListarWorflows();
    }
}