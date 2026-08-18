using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Almacenamiento.Interfaces;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorGuardarFotoIdentificacionVehicular : ProcesadorComando<GuardarFotoIdentificacionVehicular>
    {
        private readonly IAlmacenamientoFotos almacenamientoFotos;

        public ProcesadorGuardarFotoIdentificacionVehicular(IRepositorio repositorio, IConversor conversor, ILogger log,
            IAlmacenamientoFotos almacenamientoFotos)
            : base(repositorio, conversor, log)
        {
            this.almacenamientoFotos = almacenamientoFotos;
        }

        public override Resultado Ejecutar(GuardarFotoIdentificacionVehicular comando)
        {
            var resultado = new Resultado();
            try
            {
                var logIdentificacion = Repositorio.Obtener<LogIdentificacionVehicular>(comando.LogIdentificacionVehicularId);
                if (logIdentificacion == null)
                {
                    Log.Warn("No se encontró LogIdentificacionVehicular con Id {0}", comando.LogIdentificacionVehicularId);
                    return resultado;
                }

                if (logIdentificacion.PuestoDeTrabajo == null)
                {
                    Log.Warn("El LogIdentificacionVehicular con Id {0} no tiene un PuestoDeTrabajo asociado", comando.LogIdentificacionVehicularId);
                    return resultado;
                }

                var camarasDelPuesto = logIdentificacion.PuestoDeTrabajo.VideoCamaras ?? new List<VideoCamara>();
                if (!camarasDelPuesto.Any())
                {
                    Log.Warn("No hay cámaras configuradas para el puesto {0}", logIdentificacion.PuestoDeTrabajo.Id);
                    return resultado;
                }

                var detallesCoincidentes = logIdentificacion.Detalles?
                        .Where(d => 
                            !string.IsNullOrEmpty(d.RutaImagen) 
                            && !string.IsNullOrEmpty(d.CodigoCamara) 
                            && camarasDelPuesto.Any(c => 
                                string.Equals(c.Codigo, d.CodigoCamara, StringComparison.InvariantCultureIgnoreCase) 
                                && !string.IsNullOrEmpty(c.Directorio)
                            )
                        ).ToList();

                if (detallesCoincidentes == null || !detallesCoincidentes.Any())
                {
                    Log.Warn("No se encontraron detalles de identificación con imagen coincidente con las cámaras del puesto {0} para LogId {1}", logIdentificacion.PuestoDeTrabajo.Id, comando.LogIdentificacionVehicularId);
                    return resultado;
                }

                var fechaActual = DateTime.Now;
                foreach (var detalle in detallesCoincidentes)
                {
                    var camara = camarasDelPuesto.First(c => string.Equals(c.Codigo, detalle.CodigoCamara, StringComparison.InvariantCultureIgnoreCase));
                    var extension = ObtenerExtension(detalle.RutaImagen);
                    var nombreArchivo = FotoCamionHelper.GenerarNombre(comando.CentroCodigoSap, comando.NumeroDocumentoIngreso, comando.Patente, comando.ProximaActividad, fechaActual, comando.TipoVehiculo) + extension;
                    almacenamientoFotos.CopiarImagenDesdeRuta(detalle.RutaImagen, camara.Directorio, nombreArchivo, comando.Sobreescribir);
                    fechaActual = fechaActual.AddSeconds(1);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar foto de identificación vehicular. LogId {0}", comando.LogIdentificacionVehicularId);
                resultado.Error(nameof(Exception), ex.Message);
            }

            return resultado;
        }

        private string ObtenerExtension(string ruta)
        {
            var extension = Path.GetExtension(ruta);
            if (string.IsNullOrEmpty(extension))
                return ".jpeg";

            return extension;
        }
    }
}
