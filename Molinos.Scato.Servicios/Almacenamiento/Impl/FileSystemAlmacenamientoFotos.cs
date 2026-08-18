using Molinos.Scato.Servicios.Almacenamiento.Interfaces;
using Ninject.Extensions.Logging;
using System;
using System.IO;

namespace Molinos.Scato.Servicios.Almacenamiento.Impl
{
    public class FileSystemAlmacenamientoFotos : IAlmacenamientoFotos
    {
        private readonly ILogger log;

        public FileSystemAlmacenamientoFotos(ILogger log)
        {
            this.log = log;
        }

        public string CopiarImagenDesdeRuta(string rutaOrigen, string rutaDestinoDirectorio, string nombreArchivo, bool sobreescribir = true)
        {
            var rutaDestinoCompleta = string.Empty;
            try
            {
                if (!File.Exists(rutaOrigen))
                {
                    log.Warn("No se encontró el archivo origen para copiar: {0}", rutaOrigen);
                    return null;
                }

                rutaDestinoCompleta = PrepararRutaDestino(rutaDestinoDirectorio, nombreArchivo);
                File.Copy(rutaOrigen, rutaDestinoCompleta, overwrite: sobreescribir);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error al copiar la foto desde {0} a {1}", rutaOrigen, rutaDestinoCompleta);
            }

            return rutaDestinoCompleta;
        }

        private string PrepararRutaDestino(string rutaDestinoDirectorio, string nombreArchivo)
        {
            if (!Directory.Exists(rutaDestinoDirectorio))
                Directory.CreateDirectory(rutaDestinoDirectorio);

            return Path.Combine(rutaDestinoDirectorio, nombreArchivo);
        }
    }
}
