using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarCPEPorDestino : ProcesadorComando<ConsultarCPEPorDestino>
    {
        private readonly CpePortType serviceAfipCPDigital;
        private readonly IAccesoWsCtg accesoWsCtg;

        public ProcesadorConsultarCPEPorDestino(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCPDigital, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCPDigital = serviceAfipCPDigital;
        }

        public override Resultado Ejecutar(ConsultarCPEPorDestino comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new ResultadoConsultaCpePorDestino();

            var centro = Repositorio.Obtener<Centro>(comando.CentroId);
            //validar que tenga planta
            if (!centro.Planta.HasValue) 
            {
                Log.Debug("El centro " + centro.Descripcion + " no tiene configurado la planta afip");
                resultado.Errores.Add("0", "El centro " + centro.Descripcion + " no tiene configurado la planta afip");
                return resultado;
            }

            try
            {
                Log.Debug("ProcesadorConsultarCPDigital - Creo la autorizacion");
                // Obtengo la autorizacion
                var auth = accesoWsCtg.ObtenerAuth(centro.Cuit.Replace("-", string.Empty), resultado);


                var request = new consultarCPEPorDestinoRequest
                {
                    auth = auth,
                    solicitud = new ConsultarCPEPorDestinoSolicitud
                    {
                        planta = centro.Planta.Value,
                        tipoCartaPorte = comando.TipoCpe == TipoCpeConsulta.Camion ? 74 : (comando.TipoCpe == TipoCpeConsulta.Tren ? 79 : 0),
                        tipoCartaPorteSpecified = comando.TipoCpe != TipoCpeConsulta.Ambos,
                        fechaPartidaDesde = comando.FechaPartidaDesde,
                        fechaPartidaHasta = comando.FechaPartidaHasta
                    }
                };
                Log.Debug(request.ToXml());

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var responseCp = serviceAfipCPDigital.consultarCPEPorDestino(request);
                Log.Debug("Respuesta AFIP");
                Log.Debug(responseCp.ToXml());

                if (responseCp.respuesta == null)
                {
                    resultado.Errores.Add("2", "No se obtuvo respuesta desde AFIP");
                    Log.Debug($"Consulta de CPE por destino sin respuesta");
                    return resultado;
                }

                if (responseCp.respuesta != null && responseCp.respuesta.errores != null && responseCp.respuesta.errores.Any())
                {
                    foreach (var error in responseCp.respuesta.errores)
                    {
                        resultado.Errores.Add("2", error.descripcion);
                    }

                    foreach (var error in responseCp.respuesta.errores)
                    {
                        Log.Error(string.Format("ProcesadorConsultarCPEPorDestino - ({0}) {1}", error.codigo, error.descripcion));
                    }
                }

                if(responseCp?.respuesta?.cartaPorte?.Length > 0)
                {
                    resultado.Cpes = responseCp.respuesta.cartaPorte.Select(x => new CartaPorteResumenDto()
                    {
                        Ctg = x.nroCTG,
                        Estado = x.estado,
                        FechaPartida = x.fechaPartida,
                        FechaUltimaModificacion = x.fechaUltimaModificacion,
                        TipoCartaPorte = x.tipoCartaPorte
                    }).ToList();
                }
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
    }
}
