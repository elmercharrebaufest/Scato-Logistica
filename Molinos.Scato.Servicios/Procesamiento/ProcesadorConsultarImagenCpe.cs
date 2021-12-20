using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using PdfiumViewer;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarImagenCpe : ProcesadorComando<ConsultarImagenCpe>
    {
        public ProcesadorConsultarImagenCpe(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {

        }

        public override Resultado Ejecutar(ConsultarImagenCpe comando)
        {
            var resultado = new ResultadoConsultarImagenCpe();

            try
            {
                var cartaPorteElectronica = Repositorio.Listar<CartaPorteElectronica>(x => x.NroCTG == comando.NroCtg).FirstOrDefault();
                if (cartaPorteElectronica != null)
                {
                    if (cartaPorteElectronica.Pdf != null)
                    {
                        using (var document = PdfDocument.Load(new MemoryStream(cartaPorteElectronica.Pdf)))
                        {
                            var dpix = ConfigurationManager.AppSettings["PdfCpeDpiX"];
                            var dpiy = ConfigurationManager.AppSettings["PdfCpeDpiY"];

                            var image = document.Render(0, string.IsNullOrEmpty(dpix) ? 600 : Convert.ToInt32(dpix), string.IsNullOrEmpty(dpiy) ? 600 : Convert.ToInt32(dpiy), PdfRenderFlags.ForPrinting | PdfRenderFlags.CorrectFromDpi);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                resultado.PdfImage = ms.ToArray();
                            }
                        }
                    }
                    else
                    {
                        resultado.Errores.Add("3","No se pudo obtener la imagen de la CP desde SCATO");
                    }
                } 
                else
                {
                    resultado.Errores.Add("4", Textos.Error_Generico);
                }
            }
            catch (Exception ex)
            {
                resultado.Errores.Add("5", Textos.Error_Generico);
                Log.Error(ex, $"Error al obtener pdf de ctg {comando.NroCtg} en ProcesadorConsultarImagenCpe");
            }

            return resultado;
        }
    }
}
