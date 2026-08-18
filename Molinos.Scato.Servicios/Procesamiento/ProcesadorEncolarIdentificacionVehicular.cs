using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.ColasFIFO.Interfaces;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEncolarIdentificacionVehicular : ProcesadorComando<EncolarIdentificacionVehicular>
    {
        private readonly IColaIdentificacionVehicular cola;

        public ProcesadorEncolarIdentificacionVehicular(IRepositorio repositorio, IConversor conversor, ILogger log, IColaIdentificacionVehicular cola)
            : base(repositorio, conversor, log)
        {
            this.cola = cola;
        }

        public override Resultado Ejecutar(EncolarIdentificacionVehicular comando)
        {
            var resultado = new ResultadoEncolarIdentificacionVehicular();

            try
            {
                var logIdentificacion = Repositorio.ObtenerPrimero<LogIdentificacionVehicular>(x => x.Id == comando.LogIdentificacionVehicularId);
                if (logIdentificacion == null)
                {
                    resultado.Error(nameof(comando.LogIdentificacionVehicularId), Constantes.ResultadoProcesoIdentificacionVehicular.LecturaSinLogId);
                    return resultado;
                }

                if (logIdentificacion.PuestoDeTrabajo == null)
                {
                    resultado.Error(nameof(comando.LogIdentificacionVehicularId), Textos.PuestoDeTrabajo_NoEncontrado);
                    return resultado;
                }

                var rutaFoto = logIdentificacion.Detalles != null 
                    ? logIdentificacion.Detalles
                        .Where(x => !string.IsNullOrEmpty(x.RutaImagen))
                        .OrderBy(x => x.Patente == logIdentificacion.Patente ? 1 
                                    : !string.IsNullOrEmpty(x.Patente) ? 2 
                                    : 3)
                        .Select(x => x.RutaImagen)
                        .FirstOrDefault()
                    : null;

                var mensajeError = string.IsNullOrEmpty(rutaFoto) ? "No se pudo leer la patente en la imagen" : null;
                var fotoBase64 = ImageHelper.ConvertirUrlABase64(rutaFoto);

                cola.Encolar(new ColaIdentificacionVehicularDto
                {
                    PuestoDeTrabajoId = logIdentificacion.PuestoDeTrabajo.Id,
                    Patente = logIdentificacion.Patente,
                    ReconocimientoExitoso = logIdentificacion.Recorrido != null,
                    MensajeError = mensajeError,
                    ImagenBase64 = fotoBase64,
                });

                resultado.PrimerElementoCola = cola.ObtenerPrimero(logIdentificacion.PuestoDeTrabajo.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al encolar la identificación vehicular");
                resultado.Error("Excepcion", ex.Message);
            }

            return resultado;
        }
    }
}
