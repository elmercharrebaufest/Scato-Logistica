using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Helpers;
using Ninject.Extensions.Logging;
using System;
using System.Net;

namespace Molinos.Scato.Servicios.Procesamiento
{
    /// <summary>
    /// Job batch de cacheo de CPE: consulta AFIP y persiste el resultado en CartaPorteElectronica.
    /// A diferencia de <see cref="ProcesadorConsultarCPDigital"/> (usado por el flujo interactivo de
    /// carga de carta de porte / Monitor CPE Cacheada), este procesador NO resuelve
    /// Proveedores/Localidad/Categoria/Chofer/Transportista (incluyendo las sincronizaciones con SAP
    /// que eso dispara) ni renderiza el PDF a PNG. Esa es la parte costosa (10+ consultas/round-trips
    /// por CTG) que no aporta valor cuando lo único que se necesita es refrescar la caché.
    /// </summary>
    public class ProcesadorCachearCPEAfip : ProcesadorComando<CachearCPEAfip>
    {
        private readonly CpePortType serviceAfipCPDigital;
        private readonly IAccesoWsCtg accesoWsCtg;

        public ProcesadorCachearCPEAfip(IRepositorio repositorio, IConversor conversor, ILogger log,
                                         CpePortType serviceAfipCPDigital, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.serviceAfipCPDigital = serviceAfipCPDigital;
            this.accesoWsCtg = accesoWsCtg;

            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public override Resultado Ejecutar(CachearCPEAfip comando)
        {
            var resultado = new ResultadoCachearCPEAfip();
            try
            {
                var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                var auth = accesoWsCtg.ObtenerAuth(centro.Cuit.Replace("-", string.Empty), resultado);
                if (resultado.HayErrores) return resultado;

                dynamic respuesta;
                if (comando.TipoVehiculo == (int)TipoVehiculo.Tren)
                {
                    respuesta = serviceAfipCPDigital.consultarCPEFerroviaria(new consultarCPEFerroviariaRequest
                    {
                        auth = auth,
                        solicitud = new ConsultarFerroviariaSolicitud { nroCTG = comando.NroCTG, nroCTGSpecified = true }
                    })?.respuesta;
                }
                else
                {
                    respuesta = serviceAfipCPDigital.consultarCPEAutomotor(new consultarCPEAutomotorRequest
                    {
                        auth = auth,
                        solicitud = new ConsultarAutomotorSolicitud { nroCTG = comando.NroCTG, nroCTGSpecified = true }
                    })?.respuesta;
                }

                if (CpeAfipRespuestaMapper.TieneErrorBloqueante(respuesta, resultado, Log))
                    return resultado;

                var entidad = CpeAfipRespuestaMapper.ConvertirResponseAFIPenCartaPorteElectronica(respuesta, Log);
                if (entidad == null)
                {
                    resultado.Error("afip", "No se pudo interpretar la respuesta de AFIP");
                    return resultado;
                }

                CartaPorteElectronicaCacheHelper.Registrar(Repositorio, Conversor, entidad);
                resultado.Guardado = true;
            }
            catch (Exception ex)
            {
                Log.Warn(ex, $"ProcesadorCachearCPEAfip: ARCA no disponible. CTG={comando.NroCTG}");
                resultado.Error("afip", "ARCA no disponible");
            }
            return resultado;
        }
    }
}
