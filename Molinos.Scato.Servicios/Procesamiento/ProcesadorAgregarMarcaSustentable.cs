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
            if(comando.SoloDibujar)
            {
                var resultadoDibujo = new ResultadoCartaPorteElectronica();
                Bitmap imagenBitmap;
                using (var ms = new MemoryStream(comando.PdfImage))
                {
                        imagenBitmap = new Bitmap(ms);
                }
                var imagenConSelloSustentable = DibujarSustentable(imagenBitmap);
                resultadoDibujo.PdfImageSustentable = ImageToByte(imagenConSelloSustentable);
                return resultadoDibujo;
            }

            var resultado = new ResultadoGuardarFoto();
            try
            {
                if (File.Exists(comando.RutaFotoCP))
                {
                    var nombreFoto = FotoCamionHelper.GenerarNombre(comando.CodigoCentroSap, comando.NroDocumento, comando.Patente) + "-sustentable.jpeg";
                    var imagenCp = new Bitmap(comando.RutaFotoCP);

                    var imagenCpSustentable = DibujarSustentable(imagenCp);
                    imagenCpSustentable.Save(Path.Combine(Path.GetDirectoryName(comando.RutaFotoCP), nombreFoto), ImageFormat.Jpeg);
                    resultado.Path = Path.Combine(Path.GetDirectoryName(comando.RutaFotoCP), nombreFoto);
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
                graphics.DrawImage(imagenSustentable, posicionImagenSustentableX, posicionImagenSustentableY, 300, 150);
            }

            return imagenCP;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static byte[] ImageToByte(Image img)
        {
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(myEncoder, 70L);
            using (var stream = new MemoryStream())
            {
                img.Save(stream, jgpEncoder, myEncoderParameters);
                return stream.ToArray();
            }
        }
    }
}
