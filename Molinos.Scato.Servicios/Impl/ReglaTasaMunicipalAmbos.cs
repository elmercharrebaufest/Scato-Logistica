using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Impl
{
    public class ReglaTasaMunicipalAmbos : IReglaTasaMunicipal
    {
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;

        public ReglaTasaMunicipalAmbos(IServicioRepositorio servicioRepositorio, ILogger logger, IRepositorio repositorio)
        {
            this.servicioRepositorio = servicioRepositorio ?? throw new ArgumentNullException(nameof(servicioRepositorio));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repositorio = repositorio;
        }
        public bool Aplica(DatosTasaMunicipal datos)
        {
            return datos.TipoOrigenDeValidacion == TipoOrigenDeValidacion.FormularioWorkflow || datos.TipoOrigenDeValidacion == TipoOrigenDeValidacion.Recorrido;
        }
        public TipoVehiculo ObtenerTipoVehiculo(DatosTasaMunicipal datos)
        {
           if(!datos.TipoVehiculo.HasValue)
            {
                throw new ArgumentException("El tipo de vehículo no puede ser nulo.", nameof(datos.TipoVehiculo));
            }
            return datos.TipoVehiculo.Value;
        }
    }
}
