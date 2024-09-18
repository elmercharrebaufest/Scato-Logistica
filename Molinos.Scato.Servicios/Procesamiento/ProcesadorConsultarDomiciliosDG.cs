using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.ServiceModel;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarDomiciliosDG : ProcesadorComando<ConsultarDomiciliosDG>
    {
        private readonly CpePortType serviceAfipCPDigital;
        private readonly IAccesoWsCtg accesoWsCtg;

        public ProcesadorConsultarDomiciliosDG(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCPDigital, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCPDigital = serviceAfipCPDigital;
        }

        public override Resultado Ejecutar(ConsultarDomiciliosDG comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new ResultadoConsultaDomiciliosDG();

            var centro = Repositorio.Obtener<Centro>(comando.CentroId);

            try
            {
                Log.Debug("ProcesadorConsultarDomiciliosDG - Creo la autorizacion");
                var auth = accesoWsCtg.ObtenerAuth(centro.Cuit.Replace("-", string.Empty), resultado);

                var request = new consultarDomiciliosPorCUITRequest
                {
                    auth = auth,
                    cuit = comando.Cuit
                };

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var responseCp = serviceAfipCPDigital.consultarDomiciliosPorCUIT(request);

                if (responseCp?.respuesta == null)
                {
                    resultado.Errores.Add("2", "No se obtuvo respuesta desde AFIP");
                    return resultado;
                }

                if (responseCp.respuesta.errores.Any())
                {
                    var error = responseCp.respuesta.errores.FirstOrDefault();
                    resultado.Errores.Add("2", $"Respuesta de AFIP, error: {error.codigo} - {error.descripcion}");
                    return resultado;
                }

                resultado.Domicilios = responseCp.respuesta.domicilio.Select(x => new DomicilioDto
                {
                    Tipo = x.tipo, 
                    Orden = x.orden,
                    Descripcion = x.descripcion
                }).ToList();
            }
            catch (FaultException e)
            {
                Log.Error(e, $"No se pudo hacer la consulta de Domicilios DG por el cuit {comando.Cuit} por error AFIP");
                resultado.Errores.Add("2", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, $"o se pudo hacer la consulta de Domicilios DG por el cuit {comando.Cuit}");
                resultado.Errores.Add("2", "Respuesta de AFIP, error: " + Textos.Error_Generico);
            }
            return resultado;
        }
    }
}