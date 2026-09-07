using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Servicios.Interfaces
{
    public interface IMarcaDeTiempo
    {
        void RegistrarPorSensor(string codigoDispositivo);

        void RegistrarIdentificacion(int puestoDeTrabajoId, string numeroDeTarjeta, string patente, TipoIdentificacionPorPuesto tipoIdentificacion);

        void RegistrarInicioConIdentificacion(int puestoDeTrabajoId, string numeroDeTarjeta, string patente, TipoIdentificacionPorPuesto tipoIdentificacion);

        void RegistrarFinPorInstanciaWorkflow(Guid instanceId, int? puestoDeTrabajoId);

        void RegistrarInicioPorInstanciaWorkflow(Guid instanceId);

        void RegistrarInicioPorGaritaIngreso(int puestoDeTrabajoId);

        void RegistrarFinPorGaritaIngreso(int puestoDeTrabajoId, TipoIdentificacionPorPuesto tipoIdentificacion);
    }
}
