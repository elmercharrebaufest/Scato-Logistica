using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
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

                    var request = new ConsultarAutomotorSolicitud()
                    {
                        nroCTG = Convert.ToInt64(comando.NumeroCartaPorte),
                        nroCTGSpecified = true
                    };

                    var responseCp = serviceAfipCpe.consultarCPEAutomotor(new consultarCPEAutomotorRequest()
                    {
                        auth = auth,
                        solicitud = request

                    });

                    if (responseCp?.respuesta?.transporte != null)
                    {
                        resultado.TarifaReferencia = Convert.ToDouble(responseCp.respuesta.transporte.tarifaReferencia);
                    }
                    return resultado;

                }
                else
                {
                    var request = new ConsultarFerroviariaSolicitud()
                    {
                        nroCTG = Convert.ToInt64(comando.NumeroCartaPorte),
                        nroCTGSpecified = true
                    };

                    var responseCp = serviceAfipCpe.consultarCPEFerroviaria(new consultarCPEFerroviariaRequest()
                    {
                        auth = auth,
                        solicitud = request
                    });

                    if (responseCp?.respuesta?.transporte != null)
                    {
                        resultado.TarifaReferencia = null;
                    }
                }
                return resultado;

            }
            catch (Exception ex)
            {
                Log.Error("Error al consutar AFIP " + ex.Message);
                resultado.Errores.Add(comando.NumeroCartaPorte, "Error al consutar AFIP");
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
