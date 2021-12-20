using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEtiquetaAuditoriaEnCartaDePorteDetalle : ProcesadorComando<EtiquetaAuditoriaEnCartaDePorteDetalle>
    {
        private readonly IFirmaProvider firmaProvider;
        public ProcesadorEtiquetaAuditoriaEnCartaDePorteDetalle(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider)
            : base(repositorio, conversor, log)
        {
            this.firmaProvider = firmaProvider;
        }

        public override Resultado Ejecutar(EtiquetaAuditoriaEnCartaDePorteDetalle comando)
        {
            var resultado = new Resultado();
            try
            {
                if (!File.Exists(comando.RutaFotoCartaDePorte))
                {
                    resultado.Errores.Add("ErrorArchivo", $"No existe el archivo {comando.RutaFotoCartaDePorte}");
                    Log.Error($"No existe el archivo {comando.RutaFotoCartaDePorte}");
                }

                var firma = firmaProvider.ObtenerFirmaSinLogo();
                var parametros = comando.Dto;

                StringBuilder sb = new StringBuilder();
                StringBuilder sbn = new StringBuilder();
                foreach (var iterar in parametros.Vagones)
                {
                    sbn.AppendLine("- Vagón           ");
                    sbn.AppendLine("");

                }
                foreach (var i in parametros.Vagones)
                {
                    sb.AppendLine("               " + i.Patente + ":");
                    sb.AppendLine("   Neto descargado " + (i.PesoBruto - i.PesoTara));
                   
                }

                string etiqueta = sb.ToString();
                string negrita = sbn.ToString();
                Bitmap imagenCP = (Bitmap)Image.FromFile(comando.RutaFotoCartaDePorte);

                float X = imagenCP.Width * 0.80f;
                float Y = imagenCP.Height * 0.25f;
                PointF posicionEtiqueta = new PointF(X, (Y-2));
                PointF posicionEtiquetaVagon = new PointF(X, Y);

                using (Graphics graphics = Graphics.FromImage(imagenCP))
                {
                    using (Font arialFont = new Font("Arial", 9))
                    {
                        var sizeEtiqueta = graphics.MeasureString(etiqueta, arialFont);
                        var rect = new RectangleF(posicionEtiqueta.X, posicionEtiqueta.Y, sizeEtiqueta.Width, sizeEtiqueta.Height);
                        graphics.FillRectangle(Brushes.White, rect);
                        using (Font arialFontBold = new Font("Arial", 9, FontStyle.Bold))
                        {
                            graphics.DrawString(negrita, arialFontBold, Brushes.Black, posicionEtiqueta);
                        }
                        graphics.DrawString(etiqueta, arialFont, Brushes.Black, posicionEtiquetaVagon);
                        
                        //if (parametros.FirmaImagen.Length > 0)
                        //{
                        //    using (var ms = (Bitmap)Image.FromStream(new MemoryStream(parametros.FirmaImagen)))
                        //    {
                        //        ms.MakeTransparent(Color.White);
                        //        graphics.DrawImage(ms, posicionEtiqueta.X + (sizeEtiqueta.Width * 0.50f), posicionEtiqueta.Y + (sizeEtiqueta.Height * 0.55f));
                        //    }
                        //}
                    }
                }
                var nuevoBitMap = new Bitmap(imagenCP);
                imagenCP.Dispose();
                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,70L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                nuevoBitMap.Save(comando.RutaFotoCartaDePorte, jgpEncoder, myEncoderParameters);

            }
            catch (Exception e)
            {
                resultado.Errores.Add("error1", e.Message);
            }
            return resultado;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
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


    }
}
