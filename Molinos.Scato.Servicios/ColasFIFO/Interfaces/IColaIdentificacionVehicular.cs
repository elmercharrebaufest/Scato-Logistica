using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Servicios.ColasFIFO.Interfaces
{
    public interface IColaIdentificacionVehicular
    {
        void Encolar(ColaIdentificacionVehicularDto elemento);
        void Desencolar(int puestoDeTrabajoId);
        ColaIdentificacionVehicularDto ObtenerPrimero(int puestoDeTrabajoId);
    }
}