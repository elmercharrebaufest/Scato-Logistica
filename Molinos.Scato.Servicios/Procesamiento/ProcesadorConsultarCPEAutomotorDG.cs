using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using PdfiumViewer;
using System;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarCPEAutomotorDG : ProcesadorComando<ConsultarCPEAutomotorDG>
    {
        private readonly CpePortType serviceAfipCPDigital;

        private readonly IAccesoWsCtg accesoWsCtg; public ProcesadorConsultarCPEAutomotorDG(IRepositorio repositorio, IConversor conversor, ILogger log,
        CpePortType serviceAfipCPDigital, IAccesoWsCtg accesoWsCtg)
        : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCPDigital = serviceAfipCPDigital;
        }

        public override Resultado Ejecutar(ConsultarCPEAutomotorDG comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            var resultado = new ResultadoConsultaCpeAutomotorDG();
            var centro = Repositorio.Obtener<Centro>(comando.CentroId);
            try
            {
                Log.Debug("ProcesadorConsultarCPEAutomotorDG - Creo la autorizacion");
                var auth = accesoWsCtg.ObtenerAuth(centro.Cuit.Replace("-", string.Empty), resultado); var request = new consultarCPEAutomotorDGRequest
                {
                    auth = auth,
                    solicitud = new ConsultarAutomotorDGSolicitud
                    {
                        nroCTG = comando.NumeroCTG,
                        nroCTGSpecified = true
                    }
                };
                Log.Debug(request.ToXml());

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var responseCp = serviceAfipCPDigital.consultarCPEAutomotorDG(request);

                Log.Debug("Respuesta AFIP");
                Log.Debug(responseCp.ToXml());

                if (responseCp?.respuesta == null)
                {
                    resultado.Errores.Add("2", "No se obtuvo respuesta desde AFIP");
                    Log.Debug($"Consulta de CPE por destino sin respuesta");
                    return resultado;
                }

                if (responseCp.respuesta.errores.Any())
                {
                    var error = responseCp.respuesta.errores.FirstOrDefault();
                    resultado.Errores.Add("2", $"{error.codigo} - {error.descripcion}");
                    return resultado;
                }

                resultado.NroCTG = responseCp.respuesta.cabecera.nroCTG;
                resultado.NroOrden = responseCp.respuesta.cabecera.nroOrden;
                resultado.Sucursal = responseCp.respuesta.cabecera.sucursal;
                resultado.CuitTransportista = formatoCuitCuit(responseCp.respuesta.transporte.cuitTransportista);
                resultado.PatenteCamion = responseCp.respuesta.transporte.dominio[0];
                resultado.PatenteAcoplado = responseCp.respuesta.transporte.dominio.Length > 1 ? responseCp.respuesta.transporte.dominio[1] : string.Empty;
                resultado.CuitChofer = responseCp.respuesta.transporte.cuitChofer;
                resultado.PesoBruto = responseCp.respuesta.datosCarga.pesoBruto;
                resultado.PesoTara = responseCp.respuesta.datosCarga.pesoTara;
                resultado.Pdf = responseCp.respuesta.pdf;
                resultado.CodigoGranario = responseCp.respuesta.datosCarga.codDerivadoGranario;
                resultado.CodigoPadre = responseCp.respuesta.datosCarga.codGrano;
                resultado.CuitOrigen = formatoCuitCuit(responseCp.respuesta.origen.cuitOrigen);
                resultado.PlantaDG = responseCp.respuesta.origen.planta;
                resultado.TipoCPE = responseCp.respuesta.cabecera.tipoCartaPorte;

                var imagen = ConvertirPDFaPNG(responseCp.respuesta.pdf);
                resultado.PdfBase = Convert.ToBase64String(imagen);
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo hacer la consulta de CPE por destino por error AFIP");
                resultado.Errores.Add("2", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo hacer la consulta de CPE por destino");
                resultado.Errores.Add("2", Textos.Error_Generico);
            }
            return resultado;
        }

        private string formatoCuitCuit(long cuitCuil)
        {
            StringBuilder cuilBuilder = new StringBuilder();

            var cuil = cuitCuil.ToString();

            for (int i = 0; i < cuil.Length; i++)
            {
                if (i == 2 || i == 10)
                {
                    cuilBuilder.Append("-" + cuil[i]);
                }
                else
                    cuilBuilder.Append(cuil[i]);
            }

            return cuilBuilder.ToString();
        }

        private byte[] ConvertirPDFaPNG(byte[] pdf)
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
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error al Convertir PDF en PNG");
                return new byte[0];
            }
        }
    }
}