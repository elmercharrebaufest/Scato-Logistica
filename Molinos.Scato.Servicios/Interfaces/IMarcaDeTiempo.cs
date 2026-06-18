using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Servicios.Interfaces
{
    public interface IMarcaDeTiempo
    {
        void RegistrarInicioPorSensor(string codigoDispositivo);

        void RegistrarFinPorSensor(string codigoDispositivo);

        void RegistrarIdentificacion(int puestoDeTrabajoId, string numeroDeTarjeta, string patente, TipoIdentificacionPorPuesto tipoIdentificacion);

        void RegistrarFinPorInstanciaWorkflow(Guid instanceId, int? puestoDeTrabajoId);

        void RegistrarInicioPorInstanciaWorkflow(Guid instanceId);
    }
}
