using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Hosting;
using Molinos.Scato.Dominio;
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
        private const string RutaSelloSustentable = "~/Images/sello-sustentable.jpg";

        public ProcesadorAgregarMarcaSustentable(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(AgregarMarcaSustentable comando)
        {
            if(comando.SoloDibujar)
            {
                var resultadoDibujo = new ResultadoCartaPorteElectronica();
                Bitmap imagenBitmap = null;
                try
                {
                    using (var ms = new MemoryStream(comando.PdfImage))
                    {
                        imagenBitmap = new Bitmap(ms);
                    }
                    var imagenConSelloSustentable = DibujarSustentable(imagenBitmap);
                    resultadoDibujo.PdfImageSustentable = ImageToByte(imagenConSelloSustentable);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error al agregar marca sustentable en modo SoloDibujar");
                    resultadoDibujo.Error("", e.Message);
                }
                finally
                {
                    imagenBitmap?.Dispose();
                }
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
            var configPosicionImagenSustentableX = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.MarcaSustentable && x.Nombre == Constantes.ConfiguracionGeneral.MarcaSustentable.PosicionImagenSustentableX);
            var posicionImagenSustentableX = configPosicionImagenSustentableX != null && !string.IsNullOrEmpty(configPosicionImagenSustentableX.Valor) ? Convert.ToInt32(configPosicionImagenSustentableX.Valor) : 0;

            var configPosicionImagenSustentableY = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.MarcaSustentable && x.Nombre == Constantes.ConfiguracionGeneral.MarcaSustentable.PosicionImagenSustentableY);
            var posicionImagenSustentableY = configPosicionImagenSustentableY != null && !string.IsNullOrEmpty(configPosicionImagenSustentableY.Valor) ? Convert.ToInt32(configPosicionImagenSustentableY.Valor) : 0;

            Bitmap imagenSustentable = CargarImagenSello();

            using (Graphics graphics = Graphics.FromImage(imagenCP))
            {
                graphics.DrawImage(imagenSustentable, posicionImagenSustentableX, posicionImagenSustentableY, 300, 150);
            }

            imagenSustentable.Dispose();

            return imagenCP;
        }

        private Bitmap CargarImagenSello()
        {
            var rutaFisica = HostingEnvironment.MapPath(RutaSelloSustentable);

            if (rutaFisica == null || !File.Exists(rutaFisica))
            {
                rutaFisica = RutaSelloSustentable.Replace("~/", "");
                if (!File.Exists(rutaFisica))
                {
                    throw new FileNotFoundException("No se encontró la imagen del sello sustentable en: " + RutaSelloSustentable);
                }
            }

            return new Bitmap(rutaFisica);
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
