using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using PdfiumViewer;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorGuardarImagenDescarga : ProcesadorComando<GuardarImagenDescarga>
    {
        private IServicioComandos servicioComandos;

        public ProcesadorGuardarImagenDescarga(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(GuardarImagenDescarga comando)
        {
            var resultado = new ResultadoCartaPorteElectronica();
            if(comando.Pdf != null)
            {
                ConvertirPDFaPNG(comando.Pdf, resultado);
                if (resultado.PdfImage != null)
                {
                    switch (comando.TipoImagen)
                    {
                        case Dominio.Enums.TipoImagen.CPESustentable:
                            GuardarImagenSustentable(comando, resultado);
                            break;
                        case Dominio.Enums.TipoImagen.CPEDG:
                            GuardarImagenDerivadoGranario(comando, resultado);
                            break;
                        default:
                            GuardarImagen(comando, resultado);
                            break;
                    }
                }
            }
            return resultado;
        }

        private void ConvertirPDFaPNG(byte[] pdf, ResultadoCartaPorteElectronica resultado)
        {
            try
            {
                using (var document = PdfDocument.Load(new MemoryStream(pdf)))
                {
                    var dpix = ConfigurationManager.AppSettings["PdfCpeDpiX"];
                    var dpiy = ConfigurationManager.AppSettings["PdfCpeDpiY"];
                    var image = document.Render(0, string.IsNullOrEmpty(dpix) ? 600 : Convert.ToInt32(dpix), string.IsNullOrEmpty(dpiy) ? 600 : Convert.ToInt32(dpiy), PdfRenderFlags.ForPrinting | PdfRenderFlags.CorrectFromDpi);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Png);
                        resultado.PdfImage = ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error al Convertir PDF en PNG");
                resultado.Errores.Add("4", "Error al Convertir PDF en JPG");
            }
        }

        private void GuardarImagenSustentable(GuardarImagenDescarga comando, ResultadoCartaPorteElectronica resultado)
        {
            try
            {
                var resultadoSustentable = servicioComandos.Ejecutar(new AgregarMarcaSustentable
                {
                    SoloDibujar = true,
                    PdfImage = resultado.PdfImage
                }) as ResultadoCartaPorteElectronica;

                if (File.Exists(comando.RutaFotoCP) && resultadoSustentable.PdfImageSustentable != null)
                {
                    resultadoSustentable.PdfImageSustentable = ExtensionesImage.Compress(resultadoSustentable.PdfImageSustentable);
                    var nombreFoto = FotoCamionHelper.GenerarNombre(comando.CodigoCentroSap, comando.NroCartaPorte, comando.Patente) + "-descargado-sustentable.jpeg";
                    var imagenCpSustentable = resultadoSustentable.PdfImageSustentable;

                    using (var ms = new MemoryStream(imagenCpSustentable))
                    {
                        var bitmapSustentable = new Bitmap(ms);
                        bitmapSustentable.Save(Path.Combine(Path.GetDirectoryName(comando.RutaFotoCP), nombreFoto), ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar imagen sustentable de descarga");
                resultado.Errores.Add("4", "Error al guardar imagen sustentable de descarga");
            }
        }

        private void GuardarImagen(GuardarImagenDescarga comando, ResultadoCartaPorteElectronica resultado)
        {
            try
            {
                if (File.Exists(comando.RutaFotoCP))
                {
                    resultado.PdfImage = ExtensionesImage.Compress(resultado.PdfImage);
                    var nombreFoto = FotoCamionHelper.GenerarNombre(comando.CodigoCentroSap, comando.NroCartaPorte, comando.Patente) + "-descargado.jpeg";
                    using (var ms = new MemoryStream(resultado.PdfImage))
                    {
                        var bitmapSustentable = new Bitmap(ms);
                        bitmapSustentable.Save(Path.Combine(Path.GetDirectoryName(comando.RutaFotoCP), nombreFoto), ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar imagen de descarga");
                resultado.Errores.Add("4", "Error al guardar imagen de descarga");
            }
        }

        private void GuardarImagenDerivadoGranario(GuardarImagenDescarga comando, ResultadoCartaPorteElectronica resultado)
        {
            try
            {
                if (File.Exists(comando.RutaFotoCP))
                {
                    resultado.PdfImage = ExtensionesImage.Compress(resultado.PdfImage);
                    var nombreFoto = FotoCamionHelper.GenerarNombre(comando.CodigoCentroSap, comando.NroCartaPorte, comando.Patente, comando.Etapa, DateTime.Now, comando.TipoVehiculo) + ".jpeg";
                    using (var ms = new MemoryStream(resultado.PdfImage))
                    {
                        var bitmapSustentable = new Bitmap(ms);
                        bitmapSustentable.Save(Path.Combine(Path.GetDirectoryName(comando.RutaFotoCP), nombreFoto), ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al guardar imagen CP de derivado granario");
                resultado.Errores.Add("4", "Error al guardar imagen CP de derivado granario");
            }
        }
    }
}
