using Molinos.Scato.Dominio.Dto;
using System;

namespace Molinos.Scato.Web.Firmware
{
    public interface IRecorridoWorkflow
    {
        DatosRecorridoDto ObtenerRecorrido(string numeroDeTarjeta, int puestoDeTrabajoId);
        ValidarProximaAccionDto ObtenerWorkflowProximaAccion(string numeroDeTarjeta, int puestoDeTrabajoId);
        ValidarProximaAccionDto ObtenerWorkflowProximaAccionConRecorrido(string numeroDeTarjeta, int puestoDeTrabajoId, DatosRecorridoDto recorrido);
    }
}
