using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConfirmacionArriboDefinitiva : ProcesadorComando<ConfirmarArriboDefinitivo>
    {
        private readonly CpePortType serviceAfipCpe;
        private readonly IAccesoWsCtg accesoWsCtg;
        private IServicioComandos servicioComandos;

        public ProcesadorConfirmacionArriboDefinitiva(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCpe, IAccesoWsCtg accesoWsCtg, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCpe = serviceAfipCpe;
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(ConfirmarArriboDefinitivo comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new Resultado();

            try
            {
                var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                if (centro == null)
                {
                    throw new Exception(String.Format(Textos.Error_Requerido, Textos.Centro));
                }
                var recorrido = Repositorio.Obtener<Recorrido>(g => g.InstanciaWorkflow == comando.WorkflowId);
                if (!recorrido.PesoBruto.HasValue || !recorrido.PesoTara.HasValue)
                {
                    Log.Error("ProcesadorConfirmacionArriboDefinitivo - PESO NO ENCONTRADO");
                    resultado.Errores.Add("CodigoDeBaja", "No se puede ejecutar la confirmacion definitiva de un camión sin peso");
                    return resultado;
                }

                Log.Debug("Creo la autorizacion");
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                // Obtengo la autorizacion
                var cuitRepresentado = centro.Cuit != null ? centro.Cuit.Replace("-", string.Empty) : string.Empty;
                var auth = accesoWsCtg.ObtenerAuth(cuitRepresentado, resultado);
                // Armo la consulta
                Log.Debug("armo consulta dependiendo del tipo de vehiculo");
                var response = new CartaPorteRespuesta();
                var request = "";

                var tipoCpe = ObtenerTipoCpe(comando.Dto.TipoVehiculo);
                if (tipoCpe == 74)
                {
                    var ctg = Convert.ToInt64(comando.Dto.NroCartaPorte);
                    var tipoCartaPorteElectronica = Repositorio.ObtenerProyeccion<CartaPorteElectronica, int?>(x => x.NroCTG == ctg, x => x.TipoCartaPorte);
                    tipoCpe = tipoCartaPorteElectronica is null ? tipoCpe : Convert.ToInt16(tipoCartaPorteElectronica);
                }

                //obtengo el estado actual
                if (tipoCpe == 74 || tipoCpe == 274)
                {
                    var consulta = serviceAfipCpe.consultarCPEAutomotor(new consultarCPEAutomotorRequest()
                    {
                        auth = auth,
                        solicitud = new ConsultarAutomotorSolicitud()
                        {
                            nroCTG = Convert.ToInt64(comando.Dto.NroCartaPorte),
                            nroCTGSpecified = true
                        }
                    });

                    if (consulta?.respuesta?.cabecera?.estado == "CN")
                    {
                        UpdateBajaCTGDefinitiva(comando.WorkflowId);
                        return resultado;
                    }
                }
                else
                {
                    var consulta = serviceAfipCpe.consultarCPEFerroviaria(new consultarCPEFerroviariaRequest()
                    {
                        auth = auth,
                        solicitud = new ConsultarFerroviariaSolicitud()
                        {
                            nroCTG = Convert.ToInt64(comando.Dto.NroCartaPorte),
                            nroCTGSpecified = true
                        }
                    });

                    if (consulta?.respuesta?.cabecera?.estado == "CN")
                    {
                        UpdateBajaCTGDefinitiva(comando.WorkflowId);
                        return resultado;
                    }
                }

                if (tipoCpe == 74 || tipoCpe == 274)
                {
                    var confirmarArriboRequest = new confirmacionDefinitivaCPEAutomotorRequest
                    {
                        auth = auth,
                        solicitud = new ConfirmacionAutomotorSolicitud
                        {
                            cuitSolicitante = long.Parse(comando.Dto.TitularCartaPorteCuil.Replace("-", "")),
                            pesoBrutoDescarga = recorrido.PesoBruto ?? 0,
                            pesoTaraDescarga = recorrido.PesoTara ?? 0,
                            cartaPorte = new AfipCPDigitalService.CartaPorte
                            {
                                nroOrden = int.Parse(comando.Dto.CTG),
                                sucursal = comando.Dto.Sucursal ?? 0,
                                tipoCPE = tipoCpe
                            },
                            //Optional
                            intervinientes = null
                        }
                    };
                    request = confirmarArriboRequest.ToXml();
                    Log.Debug("Inicio la consulta");
                    // Realizo la consulta
                    var respuesta = serviceAfipCpe.confirmacionDefinitivaCPEAutomotor(confirmarArriboRequest).respuesta;
                    if(respuesta.pdf != null)
                    {
                        servicioComandos.Ejecutar(new GuardarImagenDescarga
                        {
                            NroCartaPorte = comando.Dto.NroCartaPorte,
                            RutaFotoCP = comando.Dto.FotoRutaDestino,
                            CodigoCentroSap = centro.CodigoSAP,
                            Patente = recorrido.Patente,
                            EsSustentable = recorrido.Establecimiento != null,
                            Pdf = respuesta.pdf
                        });
                    } 
                    Log.Debug("Realizo la consulta ");
                }
                if (tipoCpe == 75)
                {
                    short codigoRamal = 5; // BELGRANO POR DEFECTO
                    if (comando.Dto.CodigoRamalAfip != null)
                    {
                        codigoRamal = (short)comando.Dto.CodigoRamalAfip;
                    }
                   
                    var confirmarArriboRequest = new confirmacionDefinitivaCPEFerroviariaRequest
                    {
                        auth = auth,
                        solicitud = new ConfirmacionFerroviariaSolicitud
                        {
                            cuitSolicitante = long.Parse(comando.Dto.TitularCartaPorteCuil.Replace("-", "")),
                            pesoBrutoDescarga = recorrido.PesoBruto ?? 0,
                            pesoTaraDescarga = recorrido.PesoTara ?? 0,
                            cartaPorte = new AfipCPDigitalService.CartaPorte
                            {
                                nroOrden = int.Parse(comando.Dto.CTG),
                                sucursal = comando.Dto.Sucursal ?? 0,
                                tipoCPE = tipoCpe
                            },
                            ramalDescarga = new Ramal
                            {
                                codigo = codigoRamal
                            }
                        }
                    };
                    request = confirmarArriboRequest.ToXml();
                    Log.Debug("Inicio la consulta");
                    // Realizo la consulta
                    var respuesta = serviceAfipCpe.confirmacionDefinitivaCPEFerroviaria(confirmarArriboRequest).respuesta;
                    if (respuesta.pdf != null)
                    {
                        servicioComandos.Ejecutar(new GuardarImagenDescarga
                        {
                            NroCartaPorte = comando.Dto.NroCartaPorte,
                            RutaFotoCP = comando.Dto.FotoRutaDestino,
                            CodigoCentroSap = centro.CodigoSAP,
                            Patente = recorrido.Patente,
                            EsSustentable = recorrido.Establecimiento != null,
                            Pdf = respuesta.pdf
                        });
                    }
                    Log.Debug("Realizo la consulta ");
                }
                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsCtg"] == "1")
                    {
                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = "ProcesadorConfirmacionArriboDefinitivo",
                            Fecha = DateTime.Now,
                            Comentario = request,
                            NombreUsuario = "",
                            WorkflowInstanceId = comando.WorkflowId,
                        });
                        Repositorio.GuardarCambios();
                    }
                   
                    UpdateBajaCTGDefinitiva(comando.WorkflowId);

                }
                catch (Exception e)
                {
                    Log.Debug("Error al loguear request Afip CTG", e.Message);
                }

                if (response != null && response.errores != null && response.errores.Any())
                {
                    resultado.Errores.Add(response.errores.FirstOrDefault().codigo, response.errores.FirstOrDefault().descripcion);
                    Log.Error("Error en la Confirmacion: {0}", response.errores.FirstOrDefault().descripcion);
                }
                else if (response != null)
                {
                    //Si no hay errores, registro la baja del CTG
                    var datos = response.cabecera;
                    Repositorio.Agregar(
                        new LogAfipCpe
                        {
                            Servicio = "ConfirmarArriboDefinitivo",
                            Consulta = request,
                            Respuesta = response.ToXml(),
                            Fecha = DateTime.Now,
                        });
                    Log.Debug("La Confirmacion Definitiva {0}-{1} procesada correctamente", comando.Dto.Sucursal, comando.Dto.CTG);
                }
                else
                {
                    Log.Error("La Confirmacion Definitiva {0}-{1} respuesta invalida", comando.Dto.Sucursal, comando.Dto.CTG);
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo hacer la Confirmacion Definitiva del codigo ctg {0} ", comando.Dto.NroCartaPorte);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo hacer la Confirmacion Definitiva del codigo ctg {0} ", comando.Dto.NroCartaPorte);
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
                case Dominio.Enums.TipoVehiculo.Bitren:
                    return 74;

                case Dominio.Enums.TipoVehiculo.Tren:
                case Dominio.Enums.TipoVehiculo.Vapor:
                    return 75;

                default:
                    return 74;
            }
        }

        private void UpdateBajaCTGDefinitiva(Guid workFlowId) {
            var bajaCtg = Repositorio.Obtener<BajaCTG>(x => x.WorkflowId == workFlowId);
            if (bajaCtg != null)
            {
                if(string.IsNullOrEmpty(bajaCtg.CodigoDeBajaDefinitivo))
                    bajaCtg.CodigoDeBajaDefinitivo = "ProcesadorConfirmacionArriboDefinitivo";
            }
            Repositorio.GuardarCambios();
        }
    }
}