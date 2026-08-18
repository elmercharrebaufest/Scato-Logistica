using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.ColasFIFO.Interfaces;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.ColasFIFO.Impl
{
    public class SqlColaIdentificacionVehicular : IColaIdentificacionVehicular
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger log;

        public SqlColaIdentificacionVehicular(IRepositorio repositorio, ILogger log)
        {
            this.repositorio = repositorio;
            this.log = log;
        }

        public void Encolar(ColaIdentificacionVehicularDto elemento)
        {
            if (elemento == null)
                throw new ArgumentNullException(nameof(elemento));

            var puesto = repositorio.Obtener<PuestoDeTrabajo>(elemento.PuestoDeTrabajoId);
            if (puesto == null)
            {
                log.Warn("No se encontró el puesto de trabajo {0} para encolar identificación vehicular", elemento.PuestoDeTrabajoId);
                return;
            }

            repositorio.Agregar(new ColaIdentificacionVehicular
            {
                PuestoDeTrabajo = puesto,
                Patente = elemento.Patente,
                ReconocimientoExitoso = elemento.ReconocimientoExitoso,
                MensajeError = elemento.MensajeError,
                Imagen = !string.IsNullOrEmpty(elemento.ImagenBase64) ? Convert.FromBase64String(elemento.ImagenBase64) : null,
                FechaEncolado = DateTime.Now
            });

            repositorio.GuardarCambios();
            log.Debug("Encolada patente {0} en puesto {1}", elemento.Patente, elemento.PuestoDeTrabajoId);
        }

        public void Desencolar(int puestoDeTrabajoId)
        {
            var primero = repositorio.ObtenerMenor<ColaIdentificacionVehicular, DateTime>(
                x => x.PuestoDeTrabajo.Id == puestoDeTrabajoId,
                x => x.FechaEncolado);

            if (primero == null) return;

            repositorio.Remover(primero);
            repositorio.GuardarCambios();
        }

        public ColaIdentificacionVehicularDto ObtenerPrimero(int puestoDeTrabajoId)
        {
            var primero = repositorio.ObtenerMenor<ColaIdentificacionVehicular, DateTime>(
                x => x.PuestoDeTrabajo.Id == puestoDeTrabajoId,
                x => x.FechaEncolado);

            return primero != null ? Mapear(primero) : null;
        }

        private static ColaIdentificacionVehicularDto Mapear(ColaIdentificacionVehicular entidad)
        {
            return new ColaIdentificacionVehicularDto
            {
                Id = entidad.Id,
                PuestoDeTrabajoId = entidad.PuestoDeTrabajo.Id,
                Patente = entidad.Patente,
                ReconocimientoExitoso = entidad.ReconocimientoExitoso,
                MensajeError = entidad.MensajeError,
                ImagenBase64 = entidad.Imagen != null ? Convert.ToBase64String(entidad.Imagen) : null,
            };
        }
    }
}
