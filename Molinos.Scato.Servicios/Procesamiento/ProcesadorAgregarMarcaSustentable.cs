using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorAgregarMarcaSustentable : ProcesadorComando<AgregarMarcaSustentable>
    {
        private readonly IConfiguracionProvider configuracion;
        public ProcesadorAgregarMarcaSustentable(IRepositorio repositorio, IConversor conversor, ILogger log, IConfiguracionProvider configuracion)
            : base(repositorio, conversor, log)
        {
            this.configuracion = configuracion;
        }

        public override Resultado Ejecutar(AgregarMarcaSustentable comando)
        {
            var resultado = new Resultado();
            try
            {
                if (File.Exists(comando.RutaFotoCP))
                {
                    var nombreFoto = FotoCamionHelper.GenerarNombre(comando.CodigoCentroSap, comando.NroDocumento, comando.Patente) + "-sustentable.png";
                    var imagenCp = new Bitmap(comando.RutaFotoCP);

                    var imagenCpSustentable = DibujarSustentable(imagenCp);
                    imagenCpSustentable.Save(Path.Combine(Path.GetDirectoryName(comando.RutaFotoCP), nombreFoto), ImageFormat.Png);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al agregar marca en ", comando.RutaFotoCP);
                if(e.InnerException != null)
                {
                    Log.Error(e.InnerException, "Error interno al agregar marca en ", comando.RutaFotoCP);
                }
                resultado.Error("", e.Message);
            }
            return resultado;
        }

        private Bitmap DibujarSustentable(Bitmap imagenCP)
        {
            Bitmap imagenSustentable;
            var posicionImagenSustentableX = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PosicionImagenSustentableX"));
            var posicionImagenSustentableY = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PosicionImagenSustentableY"));

            using (var ms = new MemoryStream(Convert.FromBase64String(ConfigurationManager.AppSettings.Get("ImagenSustentableBase64"))))
            {
                imagenSustentable = new Bitmap(ms);
            }

            using (Graphics graphics = Graphics.FromImage(imagenCP))
            {
                graphics.DrawImage(imagenSustentable, posicionImagenSustentableX, posicionImagenSustentableY);
            }

            return imagenCP;
        }
    }
}
