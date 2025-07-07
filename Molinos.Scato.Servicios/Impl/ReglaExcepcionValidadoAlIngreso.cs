using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Impl
{
    public class ReglaExcepcionValidadoAlIngreso : IReglaExcepcionTasaMunicipal
    {
        private readonly IRepositorio _repositorio;
        public ReglaExcepcionValidadoAlIngreso(IRepositorio repositorio)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio), "El servicio repositorio no puede ser nulo.");
        }
        public bool Aplica(DatosExcepcionTasaMunicipal datos)
        {
            return datos.InstanceId.HasValue && datos.InstanceId.Value !=  Guid.Empty ? true : false;
        }

        public Dictionary<TipoValidacionPagoTasaMunicipal, bool> ValidarExcepcion(DatosExcepcionTasaMunicipal datos)
        {
            Dictionary<TipoValidacionPagoTasaMunicipal, bool> resultadoValidacion = new Dictionary<TipoValidacionPagoTasaMunicipal, bool>();

            bool existePago = _repositorio.Existe<PagosTasaMunicipal>(p => p.IdInstance == datos.InstanceId.Value && !p.Disponible);

            resultadoValidacion.Add(TipoValidacionPagoTasaMunicipal.Abonado, existePago);

            return resultadoValidacion;
        }
    }
}
