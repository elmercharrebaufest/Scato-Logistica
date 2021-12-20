using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorGuardarfotoMesaDigitalizacion : ProcesadorComando<GuardarfotoMesaDigitalizacion>
    {
        private readonly IConfiguracionProvider configuracion;

        public ProcesadorGuardarfotoMesaDigitalizacion(IRepositorio repositorio, IConversor conversor, ILogger log, IConfiguracionProvider configuracion)
            : base(repositorio, conversor, log)
        {
            this.configuracion = configuracion;
        }

        public override Resultado Ejecutar(GuardarfotoMesaDigitalizacion comando)
        {
            var resultado = new ResultadoGuardarFoto();
            try
            {
                var path = configuracion.AppSettings["FotosPath"];
                if (!comando.Directorio.Contains(path))
                {
                    comando.Directorio = path + (path.EndsWith("\\") ? "" : "\\") + comando.Directorio;
                }
                var foto = Convert.FromBase64String(comando.FotoMesaDigitalizacion);
                var rutaDestino = string.Empty;
                if (!comando.EsTemporal)
                {
                    var subpath = comando.Fecha.ToString("yyyyMMdd");
                    var centroCodigoSAP = Repositorio.ObtenerProyeccion<Centro, string>(x => x.Id == comando.CentroId, x => x.CodigoSAP).Split(',')[0];
                    var nombreFile = FotoCamionHelper.GenerarNombre(centroCodigoSAP, comando.NumeroDocumentoIngreso, comando.Patente, "Mesa", comando.Fecha, comando.TipoVehiculo);
                    rutaDestino = comando.Directorio + (comando.Directorio.EndsWith("\\") ? "" : "\\") + subpath + "\\" + nombreFile + ".jpeg";
                }
                else
                {
                    var subpath = "temp";
                    var nombreFile = FotoCamionHelper.GenerarNombreTemporal(comando.NumeroDeTarjeta, comando.Fecha);
                    rutaDestino = comando.Directorio + (comando.Directorio.EndsWith("\\") ? "" : "\\") + subpath + "\\" + nombreFile + ".jpeg";
                }

                if (!Directory.Exists(Path.GetDirectoryName(rutaDestino)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(rutaDestino));
                }
                using (var outputStream = File.OpenWrite(rutaDestino))
                {
                    outputStream.Write(foto, 0, foto.Length);
                }
                resultado.Path = rutaDestino;
                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"GuardarfotoMesaDigitalizacion {comando.Directorio} {comando.CentroId} {comando.NumeroDocumentoIngreso} {comando.Patente}");
            }
            return null;
        }
    }
}
