using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorAutorizarCpe : ProcesadorComando<AutorizarCpe>
    {
        private readonly CpePortType serviceAfipCpe;
        private readonly IAccesoWsCtg accesoWsCtg;
        public ProcesadorAutorizarCpe(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCpe, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCpe = serviceAfipCpe;
        }

        public override Resultado Ejecutar(AutorizarCpe comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////
           
            var resultado = new Resultado();
            var erroresNobloqueantes = new List<string>() { "550" }; //no se pudo generar el pdf

            try
            {
                var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                if (centro == null)
                {
                    throw new Exception(String.Format(Textos.Error_Requerido, Textos.Centro));
                }

                Log.Debug("Creo la autorizacion");
                Log.Debug($"Input ProcesadorAutorizarCpe:CartaPorteDto : {JsonConvert.SerializeObject(comando.Dto)}");

                // Obtengo la autorizacion
                var cuitRepresentado = centro.Cuit != null ? centro.Cuit.Replace("-", string.Empty) : string.Empty;
                var auth = accesoWsCtg.ObtenerAuth(cuitRepresentado, resultado);
                comando.TipoCPE = ObtenerTipoCpe(comando.Vehiculo.TipoVehiculo);

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                //Consulto el número de orden actual.
                var afipNumOrden = serviceAfipCpe.consultarUltNroOrden(
                    new consultarUltNroOrdenRequest
                    {
                        auth = auth,
                        solicitud = new ConsultarUltNroOrdenSolicitud
                        {
                            sucursal = centro.Id,
                            tipoCPE = comando.TipoCPE
                        }
                    }
                );

                if (afipNumOrden?.respuesta?.errores?.Length > 0)
                {
                    Log.Error("Error al consultar último Numero de Orden CPE: {0}", afipNumOrden.respuesta?.errores?.FirstOrDefault()?.descripcion);
                    throw new Exception(afipNumOrden.respuesta?.errores?.FirstOrDefault()?.descripcion);
                }

                Log.Debug($"Se obtuvo el ultimo numero de orden : {afipNumOrden?.respuesta?.nroOrden} para el tipo CPE : {comando.TipoCPE} correctamente");
                comando.NroOrden = Convert.ToInt32(afipNumOrden.respuesta.nroOrden) + 1;

                if (comando.TipoCPE == 74)
                {
                    resultado = AltaCPEAutomotor(comando, auth, centro, cuitRepresentado);
                }
                else if (comando.TipoCPE == 75)
                {
                    resultado = AltaCPEFerroviaria(comando, auth, centro);
                } else
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

        private short ObtenerTipoCpe(Dominio.Enums.TipoVehiculo tipoVehiculo)
        {
            switch (tipoVehiculo)
            {
                case Dominio.Enums.TipoVehiculo.Camiones:
                case Dominio.Enums.TipoVehiculo.Camión:
                case Dominio.Enums.TipoVehiculo.CamiónC:
                case Dominio.Enums.TipoVehiculo.CamiónD:
                case Dominio.Enums.TipoVehiculo.CamiónE:
                    return 74;
                case Dominio.Enums.TipoVehiculo.Tren:
                case Dominio.Enums.TipoVehiculo.Bitren:
                case Dominio.Enums.TipoVehiculo.Vapor:
                    return 75;
                default:
                    return 74;
            }
        }

        private Resultado AltaCPEAutomotor(AutorizarCpe comando, Auth auth, Centro centro, string cuitRepresentado)
        {
            var resultado = new Resultado();
            var erroresNobloqueantes = new List<string>() { "550" }; //no se pudo generar el pdf
            var response = new autorizarCPEAutomotorResponse();
            var request = "";

            try
            {
                var autorizarCpeRequest = new autorizarCPEAutomotorRequest
                {
                    auth = auth,
                    solicitud = new AutorizarAutomotorSolicitud
                    {
                        cabecera = new CabeceraAutomotorSolicitud
                        {
                            cuitSolicitante = !string.IsNullOrEmpty(cuitRepresentado) ? long.Parse(cuitRepresentado) : default(long),
                            nroOrden = comando.NroOrden,
                            sucursal = centro.Id,
                            tipoCP = comando.TipoCPE
                        },

                        origen = new OrigenAutomotorSolicitud
                        {
                            // Optional
                            operador = new OrigenOperadorAutomotorSolicitud
                            {
                                codProvincia = Convert.ToInt32(comando?.Dto?.ProvinciaCodigoSap),
                                codLocalidad = Convert.ToInt32(comando?.Dto?.ProcedenciaCodigoSap),
                                planta = Convert.ToInt32(centro.Planta)
                            },
                            productor = null
                        },
                        correspondeRetiroProductor = false, //Consultar
                        esSolicitanteCampo = false, //Consultar
                                                    //Optional
                        retiroProductor = comando?.Dto?.RtteComercialProductorCuil is null ? null : new RetiroProductorSolicitud { cuitRemitenteComercialProductor = Convert.ToInt64(comando?.Dto?.RtteComercialProductorCuil.Replace("-", "")) },
                        //Optional
                        intervinientes = new IntervinientesSolicitud
                        {
                            cuitRemitenteComercialVentaPrimaria = Convert.ToInt64(comando?.Dto?.RtteComercialCuit?.Replace("-", "")),
                            cuitRemitenteComercialVentaPrimariaSpecified = !string.IsNullOrEmpty(comando?.Dto?.RtteComercialCuit),
                            cuitCorredorVentaSecundaria = Convert.ToInt64(comando?.Dto?.CorredorVendedorSecundarioCuil?.Replace("-", "")),
                            cuitCorredorVentaSecundariaSpecified = !string.IsNullOrEmpty(comando?.Dto?.CorredorVendedorSecundarioCuil),
                            cuitMercadoATermino = Convert.ToInt64(comando?.Dto?.AgenteComprasCuil?.Replace("-", "")),
                            cuitMercadoATerminoSpecified = !string.IsNullOrEmpty(comando?.Dto?.AgenteComprasCuil),
                            cuitCorredorVentaPrimaria = Convert.ToInt64(comando?.Dto?.CorredorVendedorCuil?.Replace("-", "")),
                            cuitCorredorVentaPrimariaSpecified = !string.IsNullOrEmpty(comando?.Dto?.CorredorVendedorCuil),
                            cuitRemitenteComercialVentaSecundaria = Convert.ToInt64(comando?.Dto?.RtteComercialVentaSecundarioCuil?.Replace("-", "")),
                            cuitRemitenteComercialVentaSecundariaSpecified = !string.IsNullOrEmpty(comando?.Dto?.RtteComercialVentaSecundarioCuil),
                            cuitRepresentanteEntregador = Convert.ToInt64(comando?.Dto?.EntregadorCuit?.Replace("-", "")),
                            cuitRepresentanteEntregadorSpecified = comando?.Dto?.Entregador?.ToUpper().Trim() == "SIN ENTREGA" ? false : !string.IsNullOrEmpty(comando?.Dto?.EntregadorCuit),
                            cuitRepresentanteRecibidor = Convert.ToInt64(comando?.Dto?.RepresentanteRecibidorCuil?.Replace("-", "")),
                            cuitRepresentanteRecibidorSpecified = !string.IsNullOrEmpty(comando?.Dto?.RepresentanteRecibidorCuil),
                            cuitRemitenteComercialVentaSecundaria2 = Convert.ToInt64(comando?.Dto?.RtteComercialVentaSecundario2Cuil?.Replace("-", "")),
                            cuitRemitenteComercialVentaSecundaria2Specified = !string.IsNullOrEmpty(comando?.Dto?.RtteComercialVentaSecundario2Cuil)
                        },
                        datosCarga = new DatosCargaAutomotorSolicitud
                        {
                            codGrano = (short)comando?.Dto?.MaterialCodigoEspecie,
                            cosecha = !string.IsNullOrEmpty(comando?.Dto?.Cosecha) ? short.Parse(comando?.Dto?.Cosecha?.Replace("-", "")) : default(short),
                            pesoBruto = comando?.Dto?.Vehiculos?.FirstOrDefault()?.PesoBrutoOrigen ?? 0,
                            pesoTara = comando?.Dto?.Vehiculos?.FirstOrDefault()?.PesoTaraOrigen ?? 0
                        },
                        destino = new DestinoSolicitud
                        {
                            cuit = !string.IsNullOrEmpty(comando?.Dto?.DestinoCuit) ? long.Parse(comando?.Dto?.DestinoCuit?.Replace("-", string.Empty)) : default(long),
                            codLocalidad = Convert.ToInt32(comando?.Dto?.DestinoLocalidadCodigoAfip),
                            codProvincia = Convert.ToInt32(comando?.Dto?.DestinoProvinciaCodigoAfip),
                            planta = Convert.ToInt32(comando?.Dto?.DestinoPlantaAfip),
                            esDestinoCampo = false, //Consultar
                            plantaSpecified = comando.Dto.DestinoPlantaAfip.HasValue
                        },
                        destinatario = new DestinatarioSolicitud
                        {
                            cuit = !string.IsNullOrEmpty(comando?.Dto?.DestinatarioCuil) ? long.Parse(comando?.Dto?.DestinatarioCuil?.Replace("-", string.Empty)) : default(long)
                        },
                        transporte = new TransporteAutomotorSolicitud
                        {
                            cuitTransportista = !string.IsNullOrEmpty(comando?.Dto?.TransportistaCUIT) ? long.Parse(comando?.Dto?.TransportistaCUIT?.Replace("-", string.Empty)) : default(long),
                            dominio = new List<string> { comando?.Vehiculo?.Patente, comando?.Vehiculo?.PatenteAcoplado }.Where(d => !string.IsNullOrEmpty(d)).ToArray(),
                            fechaHoraPartida = DateTime.Now.AddMinutes(4),
                            kmRecorrer = comando?.Dto?.KmRecorrer ?? 0,
                            codigoTurno = comando.Dto?.Cupo == "MOL1111/11111111" ? null : comando.Dto?.Cupo ?? null,
                            cuitChofer = !string.IsNullOrEmpty(comando?.Dto?.Chofer?.Cuil) ? long.Parse(comando?.Dto?.Chofer?.Cuil?.Replace("-", string.Empty)) : default(long),
                            tarifa = Convert.ToDecimal(comando?.Dto?.TarifaTonelada),
                            mercaderiaFumigada = true,
                            tarifaSpecified = !(comando?.Dto?.TarifaTonelada is null),
                            cuitIntermediarioFlete = Convert.ToInt64(comando?.Dto?.IntermediarioFleteCuil?.Replace("-", "")),
                            cuitIntermediarioFleteSpecified = !string.IsNullOrEmpty(comando?.Dto?.IntermediarioFleteCuil),
                            cuitPagadorFlete = Convert.ToInt64(comando?.Dto?.PagadorFleteCuil?.Replace("-", ""))
                        },
                        observaciones = comando?.Dto?.Observacion
                    }
                };
                request = autorizarCpeRequest.ToXml();
                Log.Debug("Inicio la consulta");
                Log.Debug($"Request Automotor : {request}");

                // Realizo la consulta
                response = serviceAfipCpe.autorizarCPEAutomotor(autorizarCpeRequest);
                Log.Debug("Realizo la consulta ");
                GuardarCPDigitalAutomor(response);

                var responseAFIP = response?.respuesta;
                if (!(responseAFIP is null))
                {
                    if (!(responseAFIP?.pdf is null))
                    {
                        responseAFIP.pdf = null;
                    }
                }

                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsCtg"] == "1")
                    {
                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = Constantes.ControlRecorrido.Actividades.Actividad,
                            Fecha = DateTime.Now,
                            Comentario = request,
                            NombreUsuario = "",
                            WorkflowInstanceId = comando.WorkflowId,
                            Mensaje = Constantes.ControlRecorrido.Mensajes.Mensaje
                        });

                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = Constantes.ControlRecorrido.Actividades.Actividad,
                            Fecha = DateTime.Now,
                            Comentario = responseAFIP is null ? string.Empty : responseAFIP.ToXml(),
                            NombreUsuario = "",
                            WorkflowInstanceId = comando.WorkflowId,
                            Mensaje = Constantes.ControlRecorrido.Mensajes.Mensaje
                        });
                        Repositorio.GuardarCambios();
                    }
                }
                catch (Exception e)
                {
                    resultado.Errores.Add("ProcesadorAutorizarCpe", e.Message);
                    Log.Debug("Error al loguear request AutorizarCpe", e.Message);
                }

                if (response != null && response?.respuesta?.errores?.Length > 0 && !erroresNobloqueantes.Any(a => response.respuesta.errores.Any(y => y.codigo == a)))
                {
                    resultado.Errores.Add(response?.respuesta?.errores?.FirstOrDefault()?.codigo, response?.respuesta?.errores?.FirstOrDefault()?.descripcion);
                    Log.Error("Error en la Autorizacion: {0}", response?.respuesta?.errores?.FirstOrDefault()?.descripcion);
                }
                else if (response != null && !string.IsNullOrEmpty(response?.respuesta?.cabecera?.nroCTG.ToString()))
                {
                    //Si no hay errores, registro alta CPE
                    Repositorio.Agregar(
                        new LogAfipCpe
                        {
                            Servicio = "ProcesadorAutorizarCpe",
                            Consulta = request,
                            Respuesta = responseAFIP is null ? string.Empty : responseAFIP.ToXml(),
                            Fecha = DateTime.Now
                        });
                    var cartaPorte = Repositorio.Obtener<Dominio.Entidades.CartaPorte>(x => x.Id == comando.Dto.Id);
                    var recorrido = Repositorio.Obtener<Dominio.Entidades.Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowId);
                    if (cartaPorte != null)
                    {
                        cartaPorte.CTG = comando.NroOrden.ToString("D8");
                        cartaPorte.NroCartaPorte = response?.respuesta?.cabecera?.nroCTG.ToString();
                        cartaPorte.Sucursal = response?.respuesta?.cabecera?.sucursal;
                        if (recorrido != null)
                        {
                            recorrido.NumeroDocumentoIngreso = response?.respuesta?.cabecera?.nroCTG.ToString();
                        }
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
                Log.Error(e, "No se pudo hacer la Autorizacion del codigo {0} ", comando.NroOrden);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
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

        private Resultado AltaCPEFerroviaria(AutorizarCpe comando, Auth auth, Centro centro)
        {
            var resultado = new Resultado();
            var erroresNobloqueantes = new List<string>() { "550" }; //no se pudo generar el pdf
            var response = new autorizarCPEFerroviariaResponse();
            var request = "";

            try
            {
                var cuitRemitenteComercialVentaPrimaria = Convert.ToInt64(comando?.Dto?.RtteComercialCuit?.Replace("-", ""));
                var cuitCorredorVentaSecundaria = Convert.ToInt64(comando?.Dto?.CorredorVendedorSecundarioCuil?.Replace("-", ""));
                var cuitMercadoATermino = Convert.ToInt64(comando?.Dto?.AgenteComprasCuil?.Replace("-", ""));
                var cuitCorredorVentaPrimaria = Convert.ToInt64(comando?.Dto?.CorredorVendedorCuil?.Replace("-", ""));
                var cuitRemitenteComercialVentaSecundaria = Convert.ToInt64(comando?.Dto?.RtteComercialVentaSecundarioCuil?.Replace("-", ""));
                var cuitRepresentanteEntregador = Convert.ToInt64(comando?.Dto?.EntregadorCuit?.Replace("-", ""));
                var cuitRepresentanteRecibidor = Convert.ToInt64(comando?.Dto?.DestinatarioCuil?.Replace("-", ""));
                var cuitRemitenteComercialVentaSecundaria2 = Convert.ToInt64(comando?.Dto?.RtteComercialVentaSecundario2Cuil?.Replace("-", ""));


                var codGrano = (short)comando?.Dto?.MaterialCodigoEspecie;
                var cosecha = !string.IsNullOrEmpty(comando?.Dto?.Cosecha) ? short.Parse(comando?.Dto?.Cosecha?.Replace("-", "")) : default(short);
                var pesoBruto = comando?.Dto?.Vehiculos?.FirstOrDefault()?.PesoBrutoOrigen ?? 0;
                var pesoTara = comando?.Dto?.Vehiculos?.FirstOrDefault()?.PesoTaraOrigen ?? 0;

                var destinocuit = !string.IsNullOrEmpty(comando?.Dto?.DestinoCuit) ? long.Parse(comando?.Dto?.DestinoCuit?.Replace("-", string.Empty)) : default(long);
                var destinoplanta = Convert.ToInt32(comando?.Dto?.DestinoPlantaAfip);
                var destinocodLocalidad = Convert.ToInt32(comando?.Dto?.DestinoLocalidadCodigoAfip);
                var destinocodProvincia = Convert.ToInt32(comando?.Dto?.DestinoProvinciaCodigoAfip);
                var destinoplantaSpecified = comando.Dto.DestinoPlantaAfip.HasValue;

                var destinatariocuit = !string.IsNullOrEmpty(comando?.Dto?.DestinatarioCuil) ? long.Parse(comando?.Dto?.DestinatarioCuil?.Replace("-", string.Empty)) : default(long);

                var transportecuitTransportista = !string.IsNullOrEmpty(comando?.Dto?.TransportistaCUIT) ? long.Parse(comando?.Dto?.TransportistaCUIT?.Replace("-", string.Empty)) : default(long);
                var transportenroVagon = Convert.ToInt32(comando?.Vehiculo?.Patente);
                var transportekmRecorrer = comando?.Dto?.KmRecorrer ?? 0;

                var nroPrecinto = comando?.Dto?.NumeroPrecinto;
                var nroPrecintoLista = new string[1];
                nroPrecintoLista[0] = nroPrecinto;

                var nroOperativo = Convert.ToInt64(comando?.Dto?.NumeroOperativo);
                var codigoRamalId = (short)comando.Dto?.CodigoRamalAfip;
                var transportecuitTransportistaTramo2 = !string.IsNullOrEmpty(comando?.Dto?.TransportistaTramo2CUIT) ? long.Parse(comando?.Dto?.TransportistaTramo2CUIT?.Replace("-", string.Empty)) : default(long);

                var confirmarArriboRequest = new autorizarCPEFerroviariaRequest
                {
                    auth = auth,
                    solicitud = new AutorizarFerroviariaSolicitud
                    {
                        cabecera = new CabeceraSolicitud
                        {
                            nroOrden = comando.NroOrden,
                            sucursal = centro.Id,
                            planta = Convert.ToInt32(centro.Planta)
                        },

                        correspondeRetiroProductor = false,
                        retiroProductor = comando?.Dto?.RtteComercialProductorCuil is null ? null : new RetiroProductorSolicitud { cuitRemitenteComercialProductor = Convert.ToInt64(comando?.Dto?.RtteComercialProductorCuil.Replace("-", "")) },
                        intervinientes = new IntervinientesSolicitud
                        {
                            cuitRemitenteComercialVentaPrimaria = cuitRemitenteComercialVentaPrimaria,
                            cuitRemitenteComercialVentaPrimariaSpecified = !string.IsNullOrEmpty(comando?.Dto?.RtteComercialCuit),
                            cuitCorredorVentaSecundaria = cuitCorredorVentaSecundaria,
                            cuitCorredorVentaSecundariaSpecified = !string.IsNullOrEmpty(comando?.Dto?.CorredorVendedorSecundarioCuil),
                            cuitMercadoATermino = cuitMercadoATermino,
                            cuitMercadoATerminoSpecified = !string.IsNullOrEmpty(comando?.Dto?.AgenteComprasCuil),
                            cuitCorredorVentaPrimaria = cuitCorredorVentaPrimaria,
                            cuitCorredorVentaPrimariaSpecified = !string.IsNullOrEmpty(comando?.Dto?.CorredorVendedorCuil),
                            cuitRemitenteComercialVentaSecundaria = cuitRemitenteComercialVentaSecundaria,
                            cuitRemitenteComercialVentaSecundariaSpecified = !string.IsNullOrEmpty(comando?.Dto?.RtteComercialVentaSecundarioCuil),
                            cuitRepresentanteEntregador = cuitRepresentanteEntregador,
                            cuitRepresentanteEntregadorSpecified = comando?.Dto?.Entregador?.ToUpper().Trim() == "SIN ENTREGA" ? false : !string.IsNullOrEmpty(comando?.Dto?.EntregadorCuit),                            
                            cuitRepresentanteRecibidor = Convert.ToInt64(comando?.Dto?.RepresentanteRecibidorCuil?.Replace("-", "")),
                            cuitRepresentanteRecibidorSpecified = !string.IsNullOrEmpty(comando?.Dto?.RepresentanteRecibidorCuil),
                            cuitRemitenteComercialVentaSecundaria2 = cuitRemitenteComercialVentaSecundaria2,
                            cuitRemitenteComercialVentaSecundaria2Specified = !string.IsNullOrEmpty(comando?.Dto?.RtteComercialVentaSecundario2Cuil)
                        },
                        datosCarga = new DatosCargaFerroviariaSolicitud
                        {
                            codGrano = codGrano,
                            cosecha = cosecha,
                            pesoBruto = pesoBruto,
                            pesoTara = pesoTara
                        },
                        destino = new DestinoSolicitud
                        {
                            cuit = destinocuit,
                            esDestinoCampo = false,
                            planta = destinoplanta,
                            codLocalidad = destinocodLocalidad,
                            codProvincia = destinocodProvincia,
                            plantaSpecified = destinoplantaSpecified
                        },
                        destinatario = new DestinatarioSolicitud
                        {
                            cuit = destinatariocuit,
                        },
                        transporte = new TransporteFerroviariaSolicitud
                        {
                            cuitTransportista = transportecuitTransportista,
                            nroVagon = transportenroVagon,
                            nroPrecinto = nroPrecintoLista,
                            nroOperativo = nroOperativo,
                            ramal = new Ramal
                            {
                                codigo = codigoRamalId
                            },
                            fechaHoraPartidaTren = DateTime.Now.AddMinutes(10),
                            kmRecorrer = transportekmRecorrer,
                            mercaderiaFumigada = true,
                            cuitTransportistaTramo2 = transportecuitTransportistaTramo2,
                            cuitTransportistaTramo2Specified = !string.IsNullOrEmpty(comando?.Dto?.TransportistaTramo2CUIT),
                            cuitPagadorFlete = Convert.ToInt64(comando?.Dto?.PagadorFleteCuil?.Replace("-", "")),
                            cuitPagadorFleteSpecified = !string.IsNullOrEmpty(comando?.Dto?.PagadorFleteCuil)
                        },
                        observaciones = comando?.Dto?.Observacion
                    }
                };
                request = confirmarArriboRequest.ToXml();
                Log.Debug("Inicio la consulta");
                Log.Debug($"Request Ferroviaria : {request}");
                // Realizo la consulta
                response = serviceAfipCpe.autorizarCPEFerroviaria(confirmarArriboRequest);

                Log.Debug("Realizo la consulta ");
                GuardarCPDigitalFerroviaria(response);

                var responseAFIP = response?.respuesta;
                if (!(responseAFIP is null))
                {
                    if (!(responseAFIP?.pdf is null))
                    {
                        responseAFIP.pdf = null;
                    }
                }

                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsCtg"] == "1")
                    {
                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = "Request Alta de CPE",
                            Fecha = DateTime.Now,
                            Comentario = request,
                            NombreUsuario = "",
                            WorkflowInstanceId = comando.WorkflowId,
                            Mensaje = "Automatico"
                        });

                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = "Response Alta de CPE",
                            Fecha = DateTime.Now,
                            Comentario = responseAFIP is null ? string.Empty : responseAFIP.ToXml(),
                            NombreUsuario = "",
                            WorkflowInstanceId = comando.WorkflowId,
                            Mensaje = "Automatico"
                        });
                        Repositorio.GuardarCambios();
                    }
                }
                catch (Exception e)
                {
                    resultado.Errores.Add("ProcesadorAutorizarCpe", e.Message);
                    Log.Debug("Error al loguear request AutorizarCpe", e.Message);
                }

                if (response != null && response?.respuesta?.errores?.Length > 0 && !erroresNobloqueantes.Any(a => response.respuesta.errores.Any(y => y.codigo == a)))
                {
                    resultado.Errores.Add(response?.respuesta?.errores?.FirstOrDefault()?.codigo, response?.respuesta?.errores?.FirstOrDefault()?.descripcion);
                    Log.Error("Error en la Autorizacion: {0}", response?.respuesta?.errores?.FirstOrDefault()?.descripcion);
                }
                else if (response != null && !string.IsNullOrEmpty(response?.respuesta?.cabecera?.nroCTG.ToString()))
                {
                    //Si no hay errores, registro alta CPE
                    Repositorio.Agregar(
                        new LogAfipCpe
                        {
                            Servicio = "ProcesadorAutorizarCpe",
                            Consulta = request,
                            Respuesta = responseAFIP is null ? string.Empty : responseAFIP.ToXml(),
                            Fecha = DateTime.Now
                        });
                    var cartaPorte = Repositorio.Obtener<Dominio.Entidades.CartaPorte>(x => x.Id == comando.Dto.Id);
                    var recorrido = Repositorio.Obtener<Dominio.Entidades.Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowId);
                    if (cartaPorte != null)
                    {
                        cartaPorte.CTG = comando.NroOrden.ToString("D8");
                        cartaPorte.NroCartaPorte = response?.respuesta?.cabecera?.nroCTG.ToString();
                        cartaPorte.Sucursal = response?.respuesta?.cabecera?.sucursal;
                        if (recorrido != null)
                        {
                            recorrido.NumeroDocumentoIngreso = response?.respuesta?.cabecera?.nroCTG.ToString();
                        }
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
                Log.Error(e, "No se pudo hacer la Autorizacion del codigo {0} ", comando.NroOrden);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
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

        private Resultado GuardarCPDigitalFerroviaria(autorizarCPEFerroviariaResponse responseCp)
        {            
            var resultado = new Resultado();
            var erroresNobloqueantes = new List<string>() { "550" }; //no se pudo generar el pdf
            var continuar = true;
            long nroCTG = 0;

            try
            {
                if (responseCp.respuesta == null)
                {
                    resultado.Errores.Add("2", "No se obtuvo respuesta desde AFIP");
                    return resultado;
                }
                nroCTG = Convert.ToInt64(responseCp?.respuesta?.cabecera?.nroCTG);
                Log.Debug("Inicio guardar Alta CPDigital Ferroviaria en tabla CartaPorteElectronica CTG {0}", nroCTG);

                if (responseCp.respuesta != null && responseCp.respuesta.errores != null && responseCp.respuesta.errores.Any())
                {
                    if (!erroresNobloqueantes.Any(x => responseCp.respuesta.errores.Any(y => y.codigo == x)))
                    {
                        foreach (var error in responseCp.respuesta.errores)
                        {
                            resultado.Errores.Add("2", error.descripcion);
                        }

                        continuar = false;
                    }

                    foreach (var error in responseCp.respuesta.errores)
                    {
                        Log.Error(string.Format("ProcesadorAutorizarCpe - ({0}) {1}", error.codigo, error.descripcion));
                    }
                }

                if (responseCp.respuesta != null && continuar)
                {
                    var cartaPorte = Repositorio.Obtener<CartaPorteElectronica>(x => x.NroCTG == nroCTG);
                    var cartaPorteRequest = new CartaPorteElectronica
                    {
                        //cabecera
                        TipoCartaPorte = responseCp.respuesta.cabecera.tipoCartaPorte,
                        Sucursal = responseCp.respuesta.cabecera.sucursal,
                        NroOrden = responseCp.respuesta.cabecera.nroOrden,
                        //Planta = responseCp.respuesta.cabecera.planta,
                        NroCTG = responseCp.respuesta.cabecera.nroCTG,
                        FechaEmision = responseCp.respuesta.cabecera.fechaEmision,
                        Estado = responseCp.respuesta.cabecera.estado,
                        FechaCP = responseCp.respuesta.cabecera.fechaInicioEstado,
                        FechaVto = responseCp.respuesta.cabecera.fechaVencimiento,

                        //origen
                        Provincia = responseCp.respuesta.origen.codProvincia,
                        Localidad = responseCp.respuesta.origen.codLocalidad,
                        Domicilio = responseCp.respuesta.origen.domicilio,
                        PlantaOrigen = responseCp.respuesta.origen.planta,

                        //correspondeRetiroProductor
                        RetiroProductor = responseCp.respuesta.correspondeRetiroProductor,

                        //retiroProductor

                        //CertificadoCOE = responseCp.respuesta.retiroProductor!= null ? responseCp.respuesta.retiroProductor.certificadoCOE: (long?)null,
                        CuitRemitenteComercialProductor = responseCp.respuesta.retiroProductor != null ? responseCp.respuesta.retiroProductor.cuitRemitenteComercialProductor : (long?)null,

                        //intervinientes
                        CuitRepresentanteRecibidor = responseCp.respuesta.intervinientes.cuitRepresentanteRecibidor,
                        CuitIntermediario = responseCp?.respuesta?.retiroProductor?.cuitRemitenteComercialProductor ?? 0,
                        CuitRemitenteComercialVentaPrimaria = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaPrimaria,
                        CuitRemitenteComercialVentaSecundaria = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaSecundaria,
                        CuitMercadoATermino = responseCp.respuesta.intervinientes.cuitMercadoATermino,
                        CuitCorredorVentaPrimaria = responseCp.respuesta.intervinientes.cuitCorredorVentaPrimaria,
                        CuitCorredorVentaSecundaria = responseCp.respuesta.intervinientes.cuitCorredorVentaSecundaria,
                        CuitRepresentanteEntregador = responseCp.respuesta.intervinientes.cuitRepresentanteEntregador,
                        CuitRemitenteComercialVentaSecundaria2 = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaSecundaria2,

                        //datosCarga
                        Material = responseCp.respuesta.datosCarga.codGrano,
                        PesoBruto = responseCp.respuesta.datosCarga.pesoBruto,
                        PesoTara = responseCp.respuesta.datosCarga.pesoTara,

                        //destino
                        CuitDestino = responseCp.respuesta.destino.cuit,
                        LocalidadDestino = responseCp.respuesta.destino.codLocalidad,
                        ProvinciaDestino = responseCp.respuesta.destino.codProvincia,
                        PlantaDestino = responseCp.respuesta.destino.planta,

                        //destinatario
                        CuitDestinatario = responseCp.respuesta.destinatario.cuit,

                        //transporte
                        CuitTransportista = responseCp.respuesta.transporte.cuitTransportista,
                        Dominio = responseCp.respuesta.transporte.nroVagon != 0 ? string.Join(",", responseCp.respuesta.transporte.nroVagon) : string.Empty,
                        FechaPartida = responseCp.respuesta.transporte.fechaHoraPartidaTren,
                        KmRecorrer = responseCp.respuesta.transporte.kmRecorrer,
                        CuitPagadorFlete = responseCp.respuesta.transporte.cuitPagadorFlete,
                        MercaderiaFumigada = responseCp.respuesta.transporte.mercaderiaFumigada,
                        CuitTransportistaTramo2 = responseCp.respuesta.transporte.cuitTransportistaTramo2,

                        //cosecha
                        Cosecha = responseCp.respuesta.datosCarga.cosecha,

                        //NroOperativo
                        NroOperativo = responseCp.respuesta.transporte.nroOperativo,

                        //Ramal
                        RamalFerroviario = responseCp?.respuesta?.transporte?.ramal?.codigo,

                        //NumeroOperativo
                        NumeroPrecinto = responseCp?.respuesta?.transporte?.nroPrecinto[0],

                        //Pdf
                        Pdf = responseCp?.respuesta?.pdf is null ? null : responseCp?.respuesta?.pdf,

                        //Onbservacion
                        Observacion = responseCp?.respuesta?.cabecera?.observaciones,

                        FechaCacheado = DateTime.Now

                    };
                    if (cartaPorte != null)
                    {
                        Conversor.Convertir(cartaPorteRequest, cartaPorte);
                        if (cartaPorteRequest.Pdf != null)
                        {
                            cartaPorte.Pdf = cartaPorteRequest.Pdf;
                        }
                    }
                    else
                    {
                        cartaPorte = Repositorio.Agregar(cartaPorteRequest);
                    }
                    Repositorio.GuardarCambios();
                    Log.Debug("Se guardo la CPE Ferroviaria correctamente CTG {0}", nroCTG);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo guardar la CPE Ferroviaria en la tabla CartaPorteElectronica CTG {0}", nroCTG);
                resultado.Errores.Add("2", Textos.Error_Generico);
            }

            return resultado;
        }

        private Resultado GuardarCPDigitalAutomor(autorizarCPEAutomotorResponse responseCp)
        {            
            var resultado = new Resultado();
            var erroresNobloqueantes = new List<string>() { "550" }; //no se pudo generar el pdf
            var continuar = true;
            long nroCTG = 0;

            try
            {
                if (responseCp.respuesta == null)
                {
                    resultado.Errores.Add("2", "No se obtuvo respuesta desde AFIP");
                    return resultado;
                }
                nroCTG = Convert.ToInt64(responseCp?.respuesta?.cabecera?.nroCTG);
                Log.Debug("Inicio guardar Alta CPDigital Automotor en tabla CartaPorteElectronica CTG {0}", nroCTG);

                if (responseCp.respuesta != null && responseCp.respuesta.errores != null && responseCp.respuesta.errores.Any())
                {
                    if (!erroresNobloqueantes.Any(x => responseCp.respuesta.errores.Any(y => y.codigo == x)))
                    {
                        foreach (var error in responseCp.respuesta.errores)
                        {
                            resultado.Errores.Add("2", error.descripcion);
                        }

                        continuar = false;
                    }

                    foreach (var error in responseCp.respuesta.errores)
                    {
                        Log.Error(string.Format("ProcesadorAutorizarCpe - ({0}) {1}", error.codigo, error.descripcion));
                    }
                }

                if (responseCp.respuesta != null && continuar)
                {
                    var cartaPorte = Repositorio.Obtener<CartaPorteElectronica>(x => x.NroCTG == nroCTG);
                    var cartaPorteRequest = new CartaPorteElectronica
                    {
                        //cabecera
                        TipoCartaPorte = responseCp.respuesta.cabecera.tipoCartaPorte,
                        Sucursal = responseCp.respuesta.cabecera.sucursal,
                        NroOrden = responseCp.respuesta.cabecera.nroOrden,
                        NroCTG = responseCp.respuesta.cabecera.nroCTG,
                        FechaEmision = responseCp.respuesta.cabecera.fechaEmision,
                        Estado = responseCp.respuesta.cabecera.estado,
                        FechaCP = responseCp.respuesta.cabecera.fechaInicioEstado,
                        FechaVto = responseCp.respuesta.cabecera.fechaVencimiento,
                        Observacion = responseCp.respuesta.cabecera.observaciones,
                        //origen
                        Provincia = responseCp.respuesta.origen.codProvincia,
                        Localidad = responseCp.respuesta.origen.codLocalidad,
                        Domicilio = responseCp.respuesta.origen.domicilio,
                        PlantaOrigen = responseCp.respuesta.origen.planta,
                        CuitOrigen = responseCp.respuesta.origen.cuit,
                        NroRenspa = responseCp.respuesta.origen.nroRenspa,

                        //correspondeRetiroProductor
                        RetiroProductor = responseCp.respuesta.correspondeRetiroProductor,

                        //retiroProductor

                        //CertificadoCOE = responseCp.respuesta.retiroProductor!= null ? responseCp.respuesta.retiroProductor.certificadoCOE: (long?)null,
                        CuitRemitenteComercialProductor = responseCp.respuesta.retiroProductor != null ? responseCp.respuesta.retiroProductor.cuitRemitenteComercialProductor : (long?)null,

                        //intervinientes
                        CuitRemitenteComercialVentaPrimaria = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaPrimaria,
                        CuitRemitenteComercialVentaSecundaria = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaSecundaria,
                        CuitIntermediario = responseCp?.respuesta?.retiroProductor?.cuitRemitenteComercialProductor ?? 0,
                        CuitMercadoATermino = responseCp.respuesta.intervinientes.cuitMercadoATermino,
                        CuitCorredorVentaPrimaria = responseCp.respuesta.intervinientes.cuitCorredorVentaPrimaria,
                        CuitCorredorVentaSecundaria = responseCp.respuesta.intervinientes.cuitCorredorVentaSecundaria,
                        CuitRepresentanteEntregador = responseCp.respuesta.intervinientes.cuitRepresentanteEntregador,
                        CuitRepresentanteRecibidor = responseCp.respuesta.intervinientes.cuitRepresentanteRecibidor,
                        CuitRemitenteComercialVentaSecundaria2 = responseCp.respuesta.intervinientes.cuitRemitenteComercialVentaSecundaria2,

                        //datosCarga
                        Material = responseCp.respuesta.datosCarga.codGrano,
                        PesoBruto = responseCp.respuesta.datosCarga.pesoBruto,
                        PesoTara = responseCp.respuesta.datosCarga.pesoTara,
                        Cosecha = responseCp.respuesta.datosCarga.cosecha,

                        //destino
                        CuitDestino = responseCp.respuesta.destino.cuit,
                        LocalidadDestino = responseCp.respuesta.destino.codLocalidad,
                        ProvinciaDestino = responseCp.respuesta.destino.codProvincia,
                        PlantaDestino = responseCp.respuesta.destino.planta,

                        //destinatario
                        CuitDestinatario = responseCp.respuesta.destinatario.cuit,

                        //transporte
                        CuitTransportista = responseCp.respuesta.transporte.cuitTransportista,
                        Dominio = responseCp.respuesta.transporte.dominio != null && responseCp.respuesta.transporte.dominio.Length > 0 ? string.Join(",", responseCp.respuesta.transporte.dominio) : string.Empty,
                        FechaPartida = responseCp.respuesta.transporte.fechaHoraPartida,
                        KmRecorrer = responseCp.respuesta.transporte.kmRecorrer,
                        CodigoTurno = responseCp.respuesta.transporte.codigoTurno,

                        CuitChofer = responseCp.respuesta.transporte.cuitChofer,
                        Tarifa = Convert.ToDouble(responseCp.respuesta.transporte.tarifa),
                        CuitPagadorFlete = responseCp.respuesta.transporte.cuitPagadorFlete,
                        CuitIntermediarioFlete = responseCp.respuesta.transporte.cuitIntermediarioFlete,
                        MercaderiaFumigada = responseCp.respuesta.transporte.mercaderiaFumigada,
                        Pdf = responseCp?.respuesta?.pdf is null ? null : responseCp?.respuesta?.pdf,
                        TarifaReferencia = Convert.ToDouble(responseCp.respuesta.transporte.tarifaReferencia),
                        FechaCacheado = DateTime.Now

                    };
                    if (cartaPorte != null)
                    {
                        Conversor.Convertir(cartaPorteRequest, cartaPorte);
                        if (cartaPorteRequest.Pdf != null)
                        {
                            cartaPorte.Pdf = cartaPorteRequest.Pdf;
                        }
                    }
                    else
                    {
                        cartaPorte = Repositorio.Agregar(cartaPorteRequest);
                    }
                    Repositorio.GuardarCambios();
                    Log.Debug("Se guardo la CPE Automotor correctamente CTG {0}", nroCTG);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo guardar la CPE Automotor en la tabla CartaPorteElectronica CTG {0}", nroCTG);
                resultado.Errores.Add("2", Textos.Error_Generico);
            }

            return resultado;
        }
    }
}