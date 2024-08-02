using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
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
    public class ProcesadorAutorizarCpeDGDummy : ProcesadorComando<AutorizarCpeDGDummy>
    {
        private readonly CpePortType serviceAfipCpe;
        private readonly IAccesoWsCtg accesoWsCtg;

        public ProcesadorAutorizarCpeDGDummy(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCpe, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCpe = serviceAfipCpe;
        }

        public override Resultado Ejecutar(AutorizarCpeDGDummy comando)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);

            var resultado = new ResultadoCartaPorteElectronicaDummy();

            try
            {
                ValidarDatosNecesarios(resultado, comando);
                if (resultado.HayErrores)
                {
                    return resultado;
                }
                var centro = Repositorio.Obtener<Centro>(x => x.Id == comando.CentroId);

                Log.Debug("Creo la autorizacion");

                // Obtengo la autorizacion
                var cuitRepresentado = centro.Cuit != null ? centro.Cuit.Replace("-", string.Empty) : string.Empty;
                var auth = accesoWsCtg.ObtenerAuth(cuitRepresentado, resultado);
                comando.TipoCPE = ObtenerTipoCpeDG(comando.TipoVehiculo);

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                //Consulto el número de orden actual.
                var afipNumOrden = serviceAfipCpe.consultarUltNroOrden(
                    new consultarUltNroOrdenRequest
                    {
                        auth = auth,
                        solicitud = new ConsultarUltNroOrdenSolicitud
                        {
                            sucursal = comando.CentroId,
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
                    resultado = AltaCPEAutomotorDG(comando, auth, centro);
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

        private ResultadoCartaPorteElectronicaDummy AltaCPEAutomotorDG(AutorizarCpeDGDummy comando, Auth auth, Centro centro)
        {
            var resultado = new ResultadoCartaPorteElectronicaDummy();
            var response = new autorizarCPEAutomotorDGResponse();
            var request = "";

            try
            {
                var material = Repositorio.Obtener<Material>(x => x.Id == comando.MaterialId);
                var pesoMax = Repositorio.Obtener<PesoMaximoPorTipoVehiculo>(x => x.Activo == true && x.TipoVehiculo == comando.TipoVehiculo && x.Centro.Id == comando.CentroId);
                var transportista = Repositorio.Obtener<Transportista>(x => x.Id == comando.TransportistaId);

                var pagadorFlete = Repositorio.Obtener<Cliente>(x => x.Id == comando.PagadorFleteId);
                var destino = Repositorio.Obtener<Cliente>(x => x.Id == comando.DestinoId);

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
                            pesoBruto = pesoMax.PesoMaxEgreso,
                            pesoTara = 1,
                            tipoEmbalaje = short.Parse(material.TipoEmbalaje.Codigo),
                            unidadMedida = 1 // Siempre es Kg
                        },
                        destino = new DestinoAutomotorDGSolicitud
                        {
                            planta = comando.DestinoPlanta,
                            cuit = !string.IsNullOrEmpty(destino.Cuit) ? long.Parse(destino.Cuit.Replace("-", string.Empty)) : 0,
                            domicilioDestino = new DomicilioPUC
                            {
                                tipo = comando.DestinoDomicilioTipo,
                                orden = comando.DestinoDomicilioOrden
                            },
                        },
                        destinatario = new DestinatarioSolicitud(),
                        transporte = new TransporteAutomotorDGSolicitud
                        {
                            cuitTransportista = !string.IsNullOrEmpty(transportista.Cuit) ? long.Parse(transportista.Cuit.Replace("-", string.Empty)) : 0,
                            dominio = comando.Dominios,
                            fechaHoraPartida = DateTime.Now.AddMinutes(10),
                            kmRecorrer = comando.KmRecorrer,
                            cuitChofer = !string.IsNullOrEmpty(comando.ChoferCuit) ? long.Parse(comando.ChoferCuit.Replace("-", string.Empty)) : 0,
                            cuitPagadorFlete = !string.IsNullOrEmpty(pagadorFlete.Cuit) ? long.Parse(pagadorFlete.Cuit.Replace("-", string.Empty)) : 0,
                        },
                        intervinientes = new IntervinientesAutomotorDGSolicitud(),
                        observaciones = comando.Observaciones
                    }
                };

                if (comando.ComisionistaId.HasValue && Repositorio.Existe<Cliente>(x => x.Id == comando.ComisionistaId))
                {
                    var comisionista = Repositorio.Obtener<Cliente>(x => x.Id == comando.ComisionistaId);
                    autorizarCpeRequest.solicitud.intervinientes.cuitComisionista = !string.IsNullOrEmpty(comisionista.Cuit) ? long.Parse(comisionista.Cuit.Replace("-", string.Empty)) : 0;
                    autorizarCpeRequest.solicitud.intervinientes.cuitComisionistaSpecified = true;
                }
                else if (comando.RemitenteId.HasValue && Repositorio.Existe<Cliente>(x => x.Id == comando.RemitenteId))
                {
                    var remitente = Repositorio.Obtener<Cliente>(x => x.Id == comando.RemitenteId);
                    autorizarCpeRequest.solicitud.intervinientes.cuitRemitenteComercial = !string.IsNullOrEmpty(remitente.Cuit) ? long.Parse(remitente.Cuit.Replace("-", string.Empty)) : 0;
                    autorizarCpeRequest.solicitud.intervinientes.cuitRemitenteComercialSpecified = true;
                }

                if (comando.AplicaDestinatario)
                {
                    var destinatario = Repositorio.Obtener<Cliente>(x => x.Id == comando.DestinatarioId);
                    autorizarCpeRequest.solicitud.destinatario.cuit = !string.IsNullOrEmpty(destinatario.Cuit) ? long.Parse(destinatario.Cuit.Replace("-", string.Empty)) : 0;
                }
                else
                {
                    var destinatario = Repositorio.Obtener<Cliente>(x => x.Id == comando.DestinoId);
                    autorizarCpeRequest.solicitud.destinatario.cuit = !string.IsNullOrEmpty(destinatario.Cuit) ? long.Parse(destinatario.Cuit.Replace("-", string.Empty)) : 0;
                }

                if (comando.CorredorId.HasValue && Repositorio.Existe<Proveedor>(x => x.Id == comando.CorredorId))
                {
                    var proveedor = Repositorio.Obtener<Proveedor>(x => x.Id == comando.CorredorId);
                    autorizarCpeRequest.solicitud.intervinientes.cuitCorredor = !string.IsNullOrEmpty(proveedor.Cuil) ? long.Parse(proveedor.Cuil.Replace("-", string.Empty)) : 0;
                    autorizarCpeRequest.solicitud.intervinientes.cuitCorredorSpecified = true;
                }

                if (comando.IntermediarioFleteId.HasValue && Repositorio.Existe<Proveedor>(x => x.Id == comando.IntermediarioFleteId))
                {
                    var intermediarioFlete = Repositorio.Obtener<Proveedor>(x => x.Id == comando.IntermediarioFleteId);
                    autorizarCpeRequest.solicitud.transporte.cuitIntermediarioFlete = !string.IsNullOrEmpty(intermediarioFlete.Cuil) ? long.Parse(intermediarioFlete.Cuil.Replace("-", string.Empty)) : 0;
                    autorizarCpeRequest.solicitud.transporte.cuitIntermediarioFleteSpecified = true;
                }

                request = autorizarCpeRequest.ToXml();
                Log.Debug($"Request Automotor : {request}");

                // Realizo la consulta
                response = serviceAfipCpe.autorizarCPEAutomotorDG(autorizarCpeRequest);
                Log.Debug("Realizo la consulta ");

                if (response != null && response?.respuesta?.errores?.Length > 0)
                {
                    resultado.Errores.Add(response?.respuesta?.errores?.FirstOrDefault()?.codigo, response?.respuesta?.errores?.FirstOrDefault()?.descripcion);
                    Log.Error("Error en la Autorizacion: {0}", response?.respuesta?.errores?.FirstOrDefault()?.descripcion);
                }
                else if (response != null && !string.IsNullOrEmpty(response?.respuesta?.cabecera?.nroCTG.ToString()))
                {
                    var responseAFIP = response?.respuesta;
                    resultado.NroOrden = responseAFIP.cabecera.nroOrden;
                    resultado.Sucursal = responseAFIP.cabecera.sucursal;
                    resultado.TipoCPE = responseAFIP.cabecera.tipoCartaPorte;
                    Repositorio.Agregar(
                        new LogAfipCpe
                        {
                            Servicio = "ProcesadorAutorizarCpeDGDummy",
                            Consulta = request,
                            Respuesta = responseAFIP is null ? string.Empty : responseAFIP.ToXml(),
                            Fecha = DateTime.Now
                        });

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
                Log.Error(e, "Ocurrió un error con AFIP al intentar autorizarCPEAutomotorDGDummy para el numero de orden {0} ", comando.NroOrden);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar autorizarCPEAutomotorDG para el numero de orden {0} ", comando.NroOrden);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
            }

            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }

            return resultado;
        }

        private void ValidarDatosNecesarios(Resultado resultado, AutorizarCpeDGDummy comando)
        {
            var centro = Repositorio.Existe<Centro>(x => x.Id == comando.CentroId);
            var material = Repositorio.Existe<Material>(x => x.Id == comando.MaterialId);
            var destino = Repositorio.Existe<Cliente>(x => x.Id == comando.DestinoId);
            var transpostista = Repositorio.Existe<Transportista>(xd => xd.Id == comando.TransportistaId);
            var pagadorFlete = Repositorio.Existe<Cliente>(x => x.Id == comando.PagadorFleteId);
            var destinatario = Repositorio.Existe<Cliente>(x => x.Id == comando.DestinatarioId);

            if (comando.AplicaDestinatario && !destinatario)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para esta alta.", Textos.Destinatario));
            }

            if (!centro)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para esta alta.", Textos.Centro));
            }

            if (!Enum.IsDefined(typeof(TipoVehiculo), comando.TipoVehiculo))
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para esta alta.", Textos.CartaPorte_TipoVehiculo));
            }

            if (!material)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para esta alta.", Textos.Material));
            }

            if (!destino)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para esta alta.", Textos.Destino));
            }

            if (!transpostista)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para esta alta.", Textos.Transportista));
            }

            if (comando.Dominios.Any(a => string.IsNullOrEmpty(a)))
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para esta alta.", Textos.Patente));
            }

            if (!pagadorFlete)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para esta alata.", Textos.CartaPorte_Transportista_Pagador_Flete));
            }
        }
    }
}