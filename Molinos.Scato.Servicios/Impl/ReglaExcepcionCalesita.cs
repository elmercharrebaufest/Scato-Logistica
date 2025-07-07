using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Impl
{
    public class ReglaExcepcionCalesita : IReglaPago24HrsTasaMunicipal
    {
        private readonly IServicioRepositorio _servicioRepositorio;

        public ReglaExcepcionCalesita(IServicioRepositorio servicioRepositorio)
        {
            _servicioRepositorio = servicioRepositorio ?? throw new ArgumentNullException(nameof(servicioRepositorio), "El servicio repositorio no puede ser nulo.");
        }

        public bool Aplica(DatosExcepcionTasaMunicipal datos)
        {
            if (datos.MaterialId.HasValue && datos.MaterialId.Value > 0)
            {
                var material = _servicioRepositorio.ObtenerMaterialPorId(datos.MaterialId.Value);
                if (material == null)
                {
                    throw new ArgumentNullException(nameof(material), "El material no puede ser nulo.");
                }

                return _servicioRepositorio.ExistePagoRealizadoPorListaMaterial(material.CodigoSAP, datos.Patente);
            }
            else
                throw new ArgumentException("El MaterialId debe ser un valor positivo.", nameof(datos.MaterialId));
        }

        public Dictionary<TipoValidacionPagoTasaMunicipal, bool> ValidarExcepcion(DatosExcepcionTasaMunicipal datos)
        {
            return new Dictionary<TipoValidacionPagoTasaMunicipal, bool>
            {
                { TipoValidacionPagoTasaMunicipal.Abonado24Hrs, true }
            };
        }

    }

 }
