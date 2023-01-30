using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorAutorizarCpeDG : ProcesadorComando<AutorizarCpeDG>
    {
        private readonly CpePortType serviceAfipCpe;
        private readonly IAccesoWsCtg accesoWsCtg;
        private readonly IServicioComandos servicioComandos;
        private readonly IConfiguracionProvider configuracion;

        public ProcesadorAutorizarCpeDG(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCpe, IAccesoWsCtg accesoWsCtg, IServicioComandos servicioComandos, IConfiguracionProvider configuracion)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCpe = serviceAfipCpe;
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
        }

        public override Resultado Ejecutar(AutorizarCpeDG comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new Resultado();

            try
            {
                ValidarDatosNecesarios(resultado, comando.WorkflowId);
                if(resultado.HayErrores)
                {
                    return resultado;
                }

                var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowId);
                
                Log.Debug("Creo la autorizacion");
                // Obtengo la autorizacion
                var cuitRepresentado = recorrido.Centro.Cuit != null ? recorrido.Centro.Cuit.Replace("-", string.Empty) : string.Empty;
                var auth = accesoWsCtg.ObtenerAuth(cuitRepresentado, resultado);
                comando.TipoCPE = ObtenerTipoCpeDG(recorrido.TipoVehiculo);

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                //Consulto el número de orden actual.
                var afipNumOrden = serviceAfipCpe.consultarUltNroOrden(
                    new consultarUltNroOrdenRequest
                    {
                        auth = auth,
                        solicitud = new ConsultarUltNroOrdenSolicitud
                        {
                            sucursal = recorrido.Centro.Id,
                            tipoCPE = comando.TipoCPE
                        }
                    }
                );

                if (afipNumOrden?.respuesta?.errores?.Length > 0)
                {
                    Log.Error("Error al consultar último Numero de Orden CPE: {0}", afipNumOrden.respuesta?.errores?.FirstOrDefault()?.descripcion);
                    throw new Exception(afipNumOrden.respuesta?.errores?.FirstOrDefault()?.descripcion);
                }

                Log.Debug($"Se obtuvo el ultimo numero de orden : {afipNumOrden?.respuesta?.nroOrden} para el tipo CPE flujo DG: {comando.TipoCPE} correctamente");
                comando.NroOrden = Convert.ToInt32(afipNumOrden.respuesta.nroOrden) + 1;

                if (comando.TipoCPE == Constantes.TipoCP.CPCamion)
                {
                    resultado = AltaCPEAutomotorDG(comando, auth, recorrido);
                }
                else
                {
                    resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo hacer la Autorizacion del codigo {0} ", comando.NroOrden);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            return resultado;
        }

        private short ObtenerTipoCpeDG(TipoVehiculo tipoVehiculo)
        {
            switch (tipoVehiculo)
            {
                case TipoVehiculo.Camión:
                case TipoVehiculo.CamiónC:
                case TipoVehiculo.CamiónD:
                case TipoVehiculo.CamiónE:
                    return Constantes.TipoCP.CPCamion;

                default:
                    return 0;
            }
        }

        private Resultado AltaCPEAutomotorDG(AutorizarCpeDG comando, Auth auth, Recorrido recorrido)
        {
            var resultado = new Resultado();
            var erroresNobloqueantes = new List<string>() { "550" }; //no se pudo generar el pdf
            var response = new autorizarCPEAutomotorDGResponse();
            var request = "";

            try
            {
                var centro = recorrido.Centro;
                var material = recorrido.Material;
                var orden = ObtenerOrdenSegunTipoDeDocumento(recorrido.TipoDocumentoIngreso, comando.WorkflowId);
                if(orden == null)
                {
                    Log.Error("No existe orden de carga para el workflow {0}", comando.WorkflowId);
                    throw new Exception("Ocurrio un error al generar la  carta de porte electronica.");
                }
                var autorizarCpeRequest = new autorizarCPEAutomotorDGRequest
                {
                    auth = auth,
                    solicitud = new AutorizarAutomotorDGSolicitud
                    {
                        cabecera = new CabeceraAutomotorDGSolicitud
                        {
                            tipoCP = comando.TipoCPE,
                            sucursal = centro.Id,
                            nroOrden = comando.NroOrden
                        },

                        origen = new OrigenAutomotorDGSolicitud
                        {
                            esUsuarioIndustria = false,
                            planta = centro.PlantaDG ?? 0,
                            plantaSpecified = true,
                            domicilioOrigen = new DomicilioPUC
                            {
                                tipo = centro.Domicilio.Tipo,
                                orden = centro.Domicilio.Orden
                            }
                        },

                        datosCarga = new DatosCargaAutomotorDGSolicitud
                        {
                            codGrano = (short)(material.CodigoGranoPadre ?? 0),
                            codDerivadoGranario = (short)(material.CodigoEspecie ?? 0),
                            pesoBruto = recorrido.PesoBruto ?? 0,
                            pesoTara = recorrido.PesoTara ?? 0,
                            tipoEmbalaje = short.Parse(material.TipoEmbalaje.Codigo),
                            unidadMedida = 1 // Siempre es Kg
                        },
                        destino = new DestinoAutomotorDGSolicitud
                        {
                            cuit = orden.DestinoCuit,
                            planta = orden.DestinoPlanta,
                            domicilioDestino = new DomicilioPUC
                            {
                                tipo = orden.DestinoDomicilioTipo,
                                orden = orden.DestinoDomicilioOrden
                            },
                        },
                        destinatario = new DestinatarioSolicitud
                        {
                            cuit = orden.DestinatarioCuit
                        },
                        transporte = new TransporteAutomotorDGSolicitud
                        {
                            cuitTransportista = !string.IsNullOrEmpty(recorrido.Transportista.Cuit) ? long.Parse(recorrido.Transportista.Cuit.Replace("-", string.Empty)) : 0,
                            dominio = orden.Dominios,
                            fechaHoraPartida = DateTime.Now.AddMinutes(10),
                            kmRecorrer = orden.KmRecorrer,
                            cuitChofer = !string.IsNullOrEmpty(recorrido.Chofer.Cuil) ? long.Parse(recorrido.Chofer.Cuil.Replace("-", string.Empty)) : 0,
                            cuitPagadorFlete = orden.PagadorFleteCuit,
                        }
                    }
                };
                request = autorizarCpeRequest.ToXml();
                Log.Debug("Inicio la consulta");
                Log.Debug($"Request Automotor : {request}");

                // Realizo la consulta
                response = serviceAfipCpe.autorizarCPEAutomotorDG(autorizarCpeRequest);
                Log.Debug("Realizo la consulta ");

                var responseAFIP = response?.respuesta;
                var rutaFotoCPE = ObtenerFotoRutaDestino();
                if (!(responseAFIP is null) && !(responseAFIP?.pdf is null))
                {
                    var resultadoGuardarImagen = servicioComandos.Ejecutar( new GuardarImagenDescarga
                    {
                        NroCartaPorte = recorrido.NumeroDocumentoIngreso,
                        RutaFotoCP = rutaFotoCPE,
                        CodigoCentroSap = recorrido.Centro.CodigoSAP,
                        Patente = recorrido.Patente,
                        TipoImagen = TipoImagen.CPEDG,
                        Pdf = responseAFIP.pdf,
                        Etapa = string.Empty,
                        TipoVehiculo = recorrido.TipoVehiculo,
                    });
                    if (resultadoGuardarImagen.HayErrores)
                    {
                        rutaFotoCPE = string.Empty;
                    }

                    responseAFIP.pdf = null;
                }

                    if (ConfigurationManager.AppSettings["LoguearRequestsCtg"] == "1")
                    {
                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = Constantes.ControlRecorrido.Mensajes.RequestAltaDG,
                            Fecha = DateTime.Now,
                            Comentario = request,
                            NombreUsuario = "",
                            WorkflowInstanceId = recorrido.InstanciaWorkflow,
                            Mensaje = Constantes.ControlRecorrido.Mensajes.Mensaje
                        });

                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = Constantes.ControlRecorrido.Mensajes.ResponseAltaDG,
                            Fecha = DateTime.Now,
                            Comentario = responseAFIP is null ? string.Empty : responseAFIP.ToXml(),
                            NombreUsuario = "",
                            WorkflowInstanceId = recorrido.InstanciaWorkflow,
                            Mensaje = Constantes.ControlRecorrido.Mensajes.Mensaje
                        });
                        Repositorio.GuardarCambios();
                    }

                if (response != null && response?.respuesta?.errores?.Length > 0 && !erroresNobloqueantes.Any(a => response.respuesta.errores.Any(y => y.codigo == a)))
                {
                    resultado.Errores.Add(response?.respuesta?.errores?.FirstOrDefault()?.codigo, response?.respuesta?.errores?.FirstOrDefault()?.descripcion);
                    Log.Error("Error en la Autorizacion: {0}", response?.respuesta?.errores?.FirstOrDefault()?.descripcion);
                }
                else if (response != null && !string.IsNullOrEmpty(response?.respuesta?.cabecera?.nroCTG.ToString()))
                {
                    Repositorio.Agregar(
                        new LogAfipCpe
                        {
                            Servicio = "ProcesadorAutorizarCpeDG",
                            Consulta = request,
                            Respuesta = responseAFIP is null ? string.Empty : responseAFIP.ToXml(),
                            Fecha = DateTime.Now
                        });
                    if(!Repositorio.Existe<CartaPorteDerivadoGranario>(x => x.Recorrido.Id == recorrido.Id))
                    {
                        var cartaPorteDerivadoGranario = new CartaPorteDerivadoGranario()
                        {
                            NroCTG = response?.respuesta?.cabecera?.nroCTG.ToString().PadLeft(12, '0'),
                            NroOrden = comando.NroOrden.ToString().PadLeft(8, '0'),
                            Sucursal = response?.respuesta?.cabecera?.sucursal.ToString().PadLeft(5, '0'),
                            RutaFotoCPEDG = rutaFotoCPE,
                            Recorrido = recorrido
                        };
                        Repositorio.Agregar(cartaPorteDerivadoGranario);
                    }
                    Log.Debug("La Autorizacion {0} procesada correctamente", comando.NroOrden);
                }
                else
                {
                    Log.Error("La Autorizacion {0} respuesta invalida", comando.NroOrden);
                    throw new Exception("Ocurrio un error al generar la  carta de porte electronica.");
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "Ocurrió un error con AFIP al intentar autorizarCPEAutomotorDG para el workflow {0} ", comando.WorkflowId);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar autorizarCPEAutomotorDG para el workflow {0} ", comando.WorkflowId);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
            }

            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }

            return resultado;
        }

        private void ValidarDatosNecesarios(Resultado resultado, Guid workflowInstance)
        {
            var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == workflowInstance);
            if (recorrido == null)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para esta orden.", Textos.Recorrido));
            }

            if (recorrido.Transportista == null)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para este recorrido.", Textos.Transportista));
            }

            if (recorrido.Material == null)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para este recorrido.", Textos.Material));
            }

            if (recorrido.Material.TipoEmbalaje == null)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para este recorrido.", Textos.Material_TipoEmbalaje));
            }

            if (recorrido.Chofer == null)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para este recorrido.", Textos.Chofer));
            }

            if (recorrido.Centro == null)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para este recorrido.", Textos.Centro));
            }

            if (recorrido.Centro.Domicilio == null)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para este centro.", Textos.Centro_Domicilio));
            }
        }

        private RequestAltaCTGDGDto ObtenerOrdenSegunTipoDeDocumento(TipoDocumentoIngreso tipoDocumento, Guid workflowInstance)
        {
            RequestAltaCTGDGDto request = null;
            switch (tipoDocumento)
            {
                case TipoDocumentoIngreso.OrdenCargaFas:
                    request = Repositorio.ObtenerConsultaEscalar(new ConsultarRequestAltaCTDGOrdenCargaFas(workflowInstance));
                    break;
                case TipoDocumentoIngreso.OrdenCargaInterna:
                    request = Repositorio.ObtenerConsultaEscalar(new ConsultarRequestAltaCTDGOrdenCargaInterna(workflowInstance));
                    break;
                case TipoDocumentoIngreso.OrdenCargaInternaFason:
                    request = Repositorio.ObtenerConsultaEscalar(new ConsultarRequestAltaCTDGOrdenCargaInternaFason(workflowInstance));
                    break;
                default:
                    break;
            }
            return request;
        }

        private string ObtenerFotoRutaDestino()
        {
            var path = configuracion.AppSettings["FotosPath"];
            return path + (path.EndsWith("\\") ? "" : "\\") + DateTime.Now.ToString("yyyyMMdd");
        }
    }
}