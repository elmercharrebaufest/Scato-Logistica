using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
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
    public class ProcesadorConsultarPlantasDG : ProcesadorComando<ConsultarPlantasDG>
    {
        private readonly CpePortType serviceAfipCPDigital;
        private readonly IAccesoWsCtg accesoWsCtg;

        public ProcesadorConsultarPlantasDG(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCPDigital, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCPDigital = serviceAfipCPDigital;
        }

        public override Resultado Ejecutar(ConsultarPlantasDG comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new ResultadoConsultaPlantasDG();

            var centro = Repositorio.Obtener<Centro>(comando.CentroId);

            try
            {
                Log.Debug("ProcesadorConsultarPlantasDG - Creo la autorizacion");
                var auth = accesoWsCtg.ObtenerAuth(centro.Cuit.Replace("-", string.Empty), resultado);

                var request = new consultarPlantasDGRequest
                {
                    auth = auth,
                    solicitud = new ConsultarPlantasDGSolicitud
                    {
                        cuit = comando.Cuit
                    }
                };

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var responseCp = serviceAfipCPDigital.consultarPlantasDG(request);

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

                resultado.Plantas = responseCp.respuesta.planta.Where(x => x.nroPlanta != 0).Select(x => x.nroPlanta).ToList();
            }
            catch (FaultException e)
            {
                Log.Error(e, $"No se pudo hacer la consulta de Plantas DG por el cuit {comando.Cuit} por error AFIP");
                resultado.Errores.Add("2", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, $"o se pudo hacer la consulta de Plantas DG por el cuit {comando.Cuit}");
                resultado.Errores.Add("2", "Respuesta de AFIP, error: " + Textos.Error_Generico);
            }
            return resultado;
        }
    }
}