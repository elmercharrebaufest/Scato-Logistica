using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Net;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarAFIP : ProcesadorComando<ConsultarAFIP>
    {
        private readonly CpePortType serviceAfipCpe;
        private readonly IAccesoWsCtg accesoWsCtg;

        public ProcesadorConsultarAFIP(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCpe, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCpe = serviceAfipCpe;
        }

        public override Resultado Ejecutar(ConsultarAFIP comando)
        {
            var resultado = new ResultadoConsultarAFIP();
            try
            {
                /////////////
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);
                //////////////

                var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                var tipoCpe = ObtenerTipoCpe(comando.TipoVehiculoId);
                var authResultado = new Resultado();
                var cuitRepresentado = centro.Cuit != null ? centro.Cuit.Replace("-", string.Empty) : string.Empty;
                var auth = accesoWsCtg.ObtenerAuth(cuitRepresentado, authResultado);

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                if (tipoCpe == 74 || tipoCpe == 274)
                {
                    ConsultaAutomotorSolicitud(comando, auth, resultado);
                }
                else
                {
                    ConsultaFerroviariaSolicitud(comando, auth, resultado);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Errores.Add("Excepcion AFIP", "Error al consultar AFIP: " + ex.Message);
                return resultado;
            }
        }

        private void ConsultaAutomotorSolicitud(ConsultarAFIP comando, Auth auth, ResultadoConsultarAFIP resultado)
        {
            var request = new ConsultarAutomotorSolicitud()
            {
                nroCTG = Convert.ToInt64(comando.CTG),
                nroCTGSpecified = true
            };

            var responseCp = serviceAfipCpe.consultarCPEAutomotor(new consultarCPEAutomotorRequest()
            {
                auth = auth,
                solicitud = request
            });

            if (responseCp != null && responseCp?.respuesta?.errores?.Length > 0)
            {
                resultado.Errores.Add(responseCp?.respuesta?.errores?.FirstOrDefault()?.codigo, responseCp?.respuesta?.errores?.FirstOrDefault()?.descripcion);
            }
            else if (responseCp?.respuesta?.transporte != null)
            {
                resultado.TarifaReferencia = Convert.ToDouble(responseCp.respuesta.transporte.tarifaReferencia);
            }
        }

        private void ConsultaFerroviariaSolicitud(ConsultarAFIP comando, Auth auth, ResultadoConsultarAFIP resultado)
        {
            var request = new ConsultarFerroviariaSolicitud()
            {
                nroCTG = Convert.ToInt64(comando.CTG),
                nroCTGSpecified = true
            };

            var responseCp = serviceAfipCpe.consultarCPEFerroviaria(new consultarCPEFerroviariaRequest()
            {
                auth = auth,
                solicitud = request
            });

            if (responseCp != null && responseCp?.respuesta?.errores?.Length > 0)
            {
                resultado.Errores.Add(responseCp?.respuesta?.errores?.FirstOrDefault()?.codigo, responseCp?.respuesta?.errores?.FirstOrDefault()?.descripcion);
            }
            else if (responseCp?.respuesta?.transporte != null)
            {
                resultado.TarifaReferencia = null;
            }
        }

        private short ObtenerTipoCpe(int tipoVehiculo)
        {
            switch ((Dominio.Enums.TipoVehiculo)tipoVehiculo)
            {
                case Dominio.Enums.TipoVehiculo.Camiones:
                case Dominio.Enums.TipoVehiculo.Camión:
                case Dominio.Enums.TipoVehiculo.CamiónC:
                case Dominio.Enums.TipoVehiculo.CamiónD:
                case Dominio.Enums.TipoVehiculo.CamiónE:
                case Dominio.Enums.TipoVehiculo.Bitren:
                    return 74;

                case Dominio.Enums.TipoVehiculo.Tren:
                case Dominio.Enums.TipoVehiculo.Vapor:
                    return 75;

                default:
                    return 74;
            }
        }
    }
}