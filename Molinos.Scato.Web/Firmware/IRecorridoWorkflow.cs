using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Web.Firmware
{
    public interface IRecorridoWorkflow
    {
        ValidarProximaAccionDto ObtenerWorkflowProximaAccion(string numeroDeTarjeta, int puestoDeTrabajoId);
        ValidarProximaAccionDto ObtenerWorkflowProximaAccionConRecorrido(string numeroDeTarjeta, int puestoDeTrabajoId, DatosRecorridoDto recorrido);
    }
}
