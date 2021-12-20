using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using ZXing;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorLeerNumeroCartaPorte : ProcesadorComando<LeerNumeroCartaPorte>
    {
        public ProcesadorLeerNumeroCartaPorte(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(LeerNumeroCartaPorte comando)
        {
            var resultado = new ResultadoLeerNumeroCartaPorte();
            IBarcodeReader reader = new BarcodeReader();

            try
            {
                using (MemoryStream mStream = new MemoryStream(comando.CodigoBarrasCartaPorte))
                {
                    using (Image img = Image.FromStream(mStream))
                    {
                        if (comando.CalcularRecorte)
                        {
                            comando.Width = img.Width; // (img.Width / 3) + 50;
                            comando.Height = img.Height / 8;
                            comando.OffsetX = 0;// comando.Width - 100;
                            comando.OffsetY = 0;
                        }

                        using (Image imagenCortada = CropImage(img, comando.OffsetX, comando.Width, comando.OffsetY, comando.Height))
                        {
                            //imagenCortada.Save("c:/tmp/" + comando.NombreArchivo);
                            var lecturas = reader.DecodeMultiple((Bitmap)imagenCortada);
                            
                            if(lecturas != null)
                            {
                                Log.Debug("Deteccion del barcode: " + string.Join(",", lecturas.Select(x => x.Text)));
                                resultado.NumeroCartaPorte = ObtenerCP(lecturas);
                            }
                        }
                    }
                }
            }
            
            catch (Exception e)
            {
                Log.Error(e, "Error al reconocer el numero de Carta de Porte desde el Barcode para el archivo {0}", comando.NombreArchivo);
                resultado.Error("", e.Message);
            }
            return resultado;
        }

        private static string ObtenerCP(Result[] lecturas)
        {

            if (lecturas != null)
            {
                foreach (var lectura in lecturas)
                {
                    if (!string.IsNullOrEmpty(lectura?.Text) && (Regex.IsMatch(lectura.Text, "^\\d{12}$") || Regex.IsMatch(lectura.Text, "^\\d{9}$")))
                        return lectura.Text;
                }
            }

            return string.Empty;
        }

        private static Image CropImage(Image img, int? margenIzq, int? width, int? margenTop, int? height)
        {
            Bitmap bmpImage = new Bitmap(img);

            return bmpImage.Clone(new Rectangle(margenIzq ?? 0, margenTop ?? 0, width ?? img.Width, height ?? img.Height), bmpImage.PixelFormat);
        }

        private static Bitmap BinaryImage(Bitmap source, int umb)
        {
            // Bitmap con la imagen binaria
            Bitmap target = new Bitmap(source.Width, source.Height, source.PixelFormat);
            // Recorrer pixel de la imagen
            for (int i = 0; i < source.Width; i++)
            {
                for (int e = 0; e < source.Height; e++)
                {
                    // Color del pixel
                    Color col = source.GetPixel(i, e);
                    // Escala de grises
                    byte gray = (byte)(col.R * 0.3f + col.G * 0.59f + col.B * 0.11f);
                    // Blanco o negro
                    byte value = 0;
                    if (gray > umb)
                    {
                        value = 255;
                    }
                    // Asginar nuevo color
                    Color newColor = System.Drawing.Color.FromArgb(value, value, value);
                    target.SetPixel(i, e, newColor);

                }
            }

            return target;
        }

        private static Bitmap HistEq(Image source)
        {
            Bitmap img = new Bitmap(source);
            int w = img.Width;
            int h = img.Height;
            BitmapData sd = img.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytes = sd.Stride * sd.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(sd.Scan0, buffer, 0, bytes);
            img.UnlockBits(sd);
            int current = 0;
            double[] pn = new double[256];
            for (int p = 0; p < bytes; p += 4)
            {
                pn[buffer[p]]++;
            }
            for (int prob = 0; prob < pn.Length; prob++)
            {
                pn[prob] /= (w * h);
            }
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    current = y * sd.Stride + x * 4;
                    double sum = 0;
                    for (int i = 0; i < buffer[current]; i++)
                    {
                        sum += pn[i];
                    }
                    for (int c = 0; c < 3; c++)
                    {
                        result[current + c] = (byte)Math.Floor(255 * sum);
                    }
                    result[current + 3] = 255;
                }
            }
            Bitmap res = new Bitmap(w, h);
            BitmapData rd = res.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, rd.Scan0, bytes);
            res.UnlockBits(rd);
            return res;
        }
    }
}
