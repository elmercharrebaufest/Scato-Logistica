using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.AfipCTGWebService;
using Molinos.Scato.Servicios.AfipWebService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarDestinatarioCPE : ProcesadorComando<ConsultarDestinatarioCPE>
    {
        const int cLongitudMaximaCupo = 16; //Se establece el limite maximo de caracteres para el campo cupo de la consulta de CTG.
        private readonly CpePortType serviceAfipCPDigital;
        private readonly IAccesoWsCtg accesoWsCtg;

        public ProcesadorConsultarDestinatarioCPE(IRepositorio repositorio, IConversor conversor, ILogger log,
            CpePortType serviceAfipCPDigital, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCPDigital = serviceAfipCPDigital;
        }

        public override Resultado Ejecutar(ConsultarDestinatarioCPE comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////
            ///
            var resultado = new Resultado();

            var centro = Repositorio.Obtener<Centro>(comando.CentroId);
            var recorrido = Repositorio.Obtener<Recorrido>(g => g.InstanciaWorkflow == comando.WorkflowId);
            try
            {
                Log.Debug("ProcesadorConsultarCupoCTG - Creo la autorizacion");
                // Obtengo la autorizacion
                var auth = accesoWsCtg.ObtenerAuth(centro.Cuit.Replace("-", string.Empty), resultado);
                // Armo la consulta
                var request = new consultarCPEAutomotorRequest
                {
                    auth = auth,
                    solicitud = new ConsultarAutomotorSolicitud
                    {
                        nroCTG = Convert.ToInt64(comando.Dto.NroCartaPorte),
                        nroCTGSpecified = true
                    }
                };

                Log.Debug(request.ToXml());

                Log.Debug("ProcesadorConsultarDestinatarioCPE - Consulta afip (ConsultaCPDigital)");

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var response = serviceAfipCPDigital.consultarCPEAutomotor(request);

                if (response != null && response.respuesta != null && response.respuesta.errores.Length > 0)
                {
                    foreach (var error in response.respuesta.errores)
                    {
                        resultado.Errores.Add("CodigoDeBaja", string.Format("({0}){1}", error.codigo, error.descripcion));
                        Log.Error("ProcesadorConsultarDestinatarioCPE -" + string.Format("({0}){1}", error.codigo, error.descripcion));
                    }
                }
                else if (response != null && response.respuesta != null && response.respuesta.cabecera != null && (response.respuesta.cabecera.estado == "AN" || response.respuesta.cabecera.estado == "RE"))
                {
                    resultado.Errores.Add("2", "Consulta Detalle CTGE - CPE Anulado o Rechazado");
                    Log.Error("ProcesadorConsultarDestinatarioCPE - CTGE {0} Anulado o Rechazado ", response.respuesta.cabecera.nroCTG);
                }
                else if (response.respuesta != null)
                {
                    var materialStr = string.Empty;
                    var respuesta = response.respuesta;

                    if (respuesta.datosCarga.codGranoSpecified)
                    {
                        var material = Repositorio.Obtener<Material>(x => x.CodigoEspecie == respuesta.datosCarga.codGrano);
                        materialStr = material?.DescripcionCorta ?? string.Empty;
                    }

                    var entidad = new DestinatarioCTG
                        {   
                            Recorrido = recorrido,
                            CanjeRemito = "", //TODO, ver como se puede calcular
                            NumeroCCPP = string.Format("{0}{1}", respuesta.cabecera.sucursal.ToString().PadLeft(5, '0'), respuesta.cabecera.nroOrden.ToString().PadLeft(8,'0')),
                            Cosecha = respuesta.datosCarga.cosecha.ToString(),
                            CTG = respuesta.cabecera.nroCTG.ToString().PadLeft(12, '0'),
                            CuitCanjeador = respuesta.intervinientes.cuitRemitenteComercialVentaSecundaria2.ToString(),
                            CuitDestinatario = respuesta.destinatario.cuit.ToString(),
                            CuitDestino = respuesta.destino.cuit.ToString(),
                            Especie = materialStr,
                            Establecimiento = respuesta.destino.planta.ToString(),
                            Estado = respuesta.cabecera.estado == "CF" ? "Confirmado" : (respuesta.cabecera.estado == "CN" ? "Confirmación Definitiva" : string.Empty),
                            FechaConf = respuesta.cabecera.fechaInicioEstado.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                            PesoNetoCarga = respuesta.datosCarga.pesoBruto - respuesta.datosCarga.pesoTara,
                            Solicitante = respuesta.origen.cuit.ToString(),
                            Cupo = respuesta.transporte.codigoTurno,
                        };
                    Repositorio.Agregar(entidad);
                    Log.Debug("Consulta de CTGE {0} procesada correctamente", comando.Dto.CTG);
                }
                else
                {
                    resultado.Errores.Add("respuesta","No se obtuvo respuesta desde AFIP");
                    Log.Debug("Consulta de CTG {0} sin respuesta", comando.Dto.CTG);
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo consultar el ctge: {0}", comando.Dto.CTG);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo consultar el ctge {0}", comando.Dto.CTG);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            return resultado;
        }
    }
}