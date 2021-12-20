using System;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorRetransmitirTransmisionASap : ProcesadorComando<RetransmitirTransmisionASap>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly WaybillManagementPODv2 servicioMonsanto;

        public ProcesadorRetransmitirTransmisionASap(IRepositorio repositorio, IServicioComandos servicioComandos, IConversor conversor, ILogger Log, ZSDWS_SCATO servicioSap, WaybillManagementPODv2 servicioMonsanto)
            : base(repositorio, conversor, Log)
        {
            this.servicioComandos = servicioComandos;
            this.servicioSap = servicioSap;
            this.servicioMonsanto = servicioMonsanto;
        }

        public override Resultado Ejecutar(RetransmitirTransmisionASap comando)
        {
            var resultado = new ResultadoRetransmitirTransmisionASap();

            Log.Debug("Obteniendo transmision id: " + comando.Id);
            var transmision = Repositorio.ObtenerConsultaEscalar(new ObtenerTransmisionASap(comando.Id));
            transmision.Estado = EstadoTransmisionASap.Error; //POR DEFECTO ASIGNO ERROR
            transmision.Id = Convert.ToInt32(comando.Id);

            switch (transmision.FuncionSap)
            {
                case FuncionSAP.AjusteDeDiferencias:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.AjusteDeDiferencias + " id: " + comando.Id);
                        var request = new MovAjusteRequest
                        {
                            MovAjuste = Conversor.Convertir<AjusteDeDiferenciasEnRedespachosTransmisionASap, MovAjuste>((AjusteDeDiferenciasEnRedespachosTransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.AjusteDeDiferencias + " id: " + comando.Id);
                        var respuestaMovAjuste = servicioSap.MovAjuste(request);
                        if (respuestaMovAjuste.MovAjusteResponse != null && respuestaMovAjuste.MovAjusteResponse.Resultado.MSGNR == "000")
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto; //LLAMADA A SAP EXISTOSA
                            transmision.MensajeError = "";
                            Log.Debug("Correcto " + FuncionSAP.AjusteDeDiferencias + " id: " + comando.Id);
                        }
                        else if (respuestaMovAjuste.MovAjusteResponse != null)
                        {
                            transmision.MensajeError = respuestaMovAjuste.MovAjusteResponse.Resultado.TEXT; //COPIO MENSAJE DE ERROR
                            Log.Debug("Error " + FuncionSAP.AjusteDeDiferencias + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                        }
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.AjusteDeDiferencias + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.LlegadaADestinosEnRedespachos:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.LlegadaADestinosEnRedespachos + " id: " + comando.Id);
                        var requestMov305 = new Mov305Request
                        {
                            Mov305 = Conversor.Convertir<LlegadaAdestinosEnRedespachosTransmisionASap, Mov305>((LlegadaAdestinosEnRedespachosTransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.LlegadaADestinosEnRedespachos + " id: " + comando.Id);
                        var respuestaMov305 = servicioSap.Mov305(requestMov305);
                        if (respuestaMov305.Mov305Response != null && !string.IsNullOrEmpty(respuestaMov305.Mov305Response.Resultado.MBLNR))
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto; //LLAMADA A SAP EXISTOSA
                            transmision.MensajeError = "";
                            Log.Debug("Correcto " + FuncionSAP.LlegadaADestinosEnRedespachos + " id: " + comando.Id);
                        }
                        else if (respuestaMov305.Mov305Response != null)
                        {
                            transmision.MensajeError = respuestaMov305.Mov305Response.Resultado.TEXT; //COPIO MENSAJE DE ERROR
                            Log.Debug("Error " + FuncionSAP.LlegadaADestinosEnRedespachos + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                        }
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.LlegadaADestinosEnRedespachos + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.SalidaDeOrigenEnRedespachos:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.SalidaDeOrigenEnRedespachos + " id: " + comando.Id);
                        var requestMov975 = new Mov975Request
                        {
                            Mov975 = Conversor.Convertir<SalidaDeOrigenEnRedespachosTransmisionASap, Mov975>((SalidaDeOrigenEnRedespachosTransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.SalidaDeOrigenEnRedespachos + " id: " + comando.Id);
                        var respuestaMov975 = servicioSap.Mov975(requestMov975);
                        if (respuestaMov975.Mov975Response != null && !string.IsNullOrEmpty(respuestaMov975.Mov975Response.Resultado.MBLNR))
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto; //LLAMADA A SAP EXISTOSA
                            transmision.MensajeError = "";
                            servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                            {
                                InstanceId = transmision.InstanciaWorkflow,
                                DocumentoInternoSap = respuestaMov975.Mov975Response.Resultado.MBLNR,
                                NumeroDeDocumentoSap = respuestaMov975.Mov975Response.Resultado.XBLNR
                            });
                            Log.Debug("Correcto " + FuncionSAP.SalidaDeOrigenEnRedespachos + " id: " + comando.Id);
                        }
                        else if (respuestaMov975.Mov975Response != null)
                        {
                            transmision.MensajeError = respuestaMov975.Mov975Response.Resultado.TEXT; //COPIO MENSAJE DE ERROR
                            Log.Debug("Error " + FuncionSAP.SalidaDeOrigenEnRedespachos + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                        }
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.SalidaDeOrigenEnRedespachos + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.IngresosPorCompraDeGranos:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.IngresosPorCompraDeGranos + " id: " + comando.Id);
                        var requestFill_Z1000 = new Fill_Z1000Request
                        {
                            Fill_Z1000 = Conversor.Convertir<IngresosPorCompraDeGranosTransmisionASap, Fill_Z1000>((IngresosPorCompraDeGranosTransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.IngresosPorCompraDeGranos + " id: " + comando.Id);
                        var respuestaZ1000 = servicioSap.Fill_Z1000(requestFill_Z1000);

                        if (respuestaZ1000.Fill_Z1000Response != null && respuestaZ1000.Fill_Z1000Response.Resultado.All(res => res.MSGNR == "000"))
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto; //LLAMADA A SAP EXISTOSA
                            transmision.MensajeError = "";
                            Log.Debug("Correcto " + FuncionSAP.IngresosPorCompraDeGranos + " id: " + comando.Id);
                        }
                        else if (respuestaZ1000.Fill_Z1000Response != null)
                        {
                            var mensaje = respuestaZ1000.Fill_Z1000Response.Resultado.First(res => res.MSGNR != "000").TEXT;
                            transmision.MensajeError = mensaje; //COPIO MENSAJE DE ERROR
                            Log.Debug("Error " + FuncionSAP.IngresosPorCompraDeGranos + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                        }
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.IngresosPorCompraDeGranos + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.EgresosMaterialNoProductivo:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.EgresosMaterialNoProductivo + " id: " + comando.Id);
                        var requestEgresos = new EgresosNoProductivosRequest
                        {
                            EgresosNoProductivos = Conversor.Convertir<EgresosNoProductivosTransmisionASap, EgresosNoProductivos>((EgresosNoProductivosTransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.EgresosMaterialNoProductivo + " id: " + comando.Id);
                        var respuestaEgresos = servicioSap.EgresosNoProductivos(requestEgresos);
                        if (respuestaEgresos.EgresosNoProductivosResponse != null && !string.IsNullOrEmpty(respuestaEgresos.EgresosNoProductivosResponse.Resultado.MBLNR))
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto; //LLAMADA A SAP EXISTOSA
                            transmision.MensajeError = "";
                            servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                            {
                                InstanceId = transmision.InstanciaWorkflow,
                                DocumentoInternoSap = respuestaEgresos.EgresosNoProductivosResponse.Resultado.MBLNR,
                                NumeroDeDocumentoSap = respuestaEgresos.EgresosNoProductivosResponse.Resultado.XBLNR
                            });
                            Log.Debug("Correcto " + FuncionSAP.EgresosMaterialNoProductivo + " id: " + comando.Id);
                        }
                        else if (respuestaEgresos.EgresosNoProductivosResponse != null)
                        {
                            transmision.MensajeError = respuestaEgresos.EgresosNoProductivosResponse.Resultado.TEXT; //COPIO MENSAJE DE ERROR
                            Log.Debug("Error " + FuncionSAP.EgresosMaterialNoProductivo + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                        }
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.EgresosMaterialNoProductivo + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.IngresosEgresosFazones:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.IngresosEgresosFazones + " id: " + comando.Id);
                        var fazonesRequest = new IngresosEgresosFazonesRequest
                        {
                            IngresosEgresosFazones = Conversor.Convertir<IngresosEgresosFazonesTransmisionASap, IngresosEgresosFazones>((IngresosEgresosFazonesTransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.IngresosEgresosFazones + " id: " + comando.Id);
                        var respuestaEgresos = servicioSap.IngresosEgresosFazones(fazonesRequest);
                        if (respuestaEgresos.IngresosEgresosFazonesResponse != null && !string.IsNullOrEmpty(respuestaEgresos.IngresosEgresosFazonesResponse.Resultado.MBLNR))
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto; //LLAMADA A SAP EXISTOSA
                            transmision.MensajeError = "";
                            servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                            {
                                InstanceId = transmision.InstanciaWorkflow,
                                DocumentoInternoSap = respuestaEgresos.IngresosEgresosFazonesResponse.Resultado.MBLNR,
                                NumeroDeDocumentoSap = respuestaEgresos.IngresosEgresosFazonesResponse.Resultado.XBLNR
                            });
                            Log.Debug("Correcto " + FuncionSAP.IngresosEgresosFazones + " id: " + comando.Id);
                        }
                        else if (respuestaEgresos.IngresosEgresosFazonesResponse != null)
                        {
                            transmision.MensajeError = respuestaEgresos.IngresosEgresosFazonesResponse.Resultado.TEXT; //COPIO MENSAJE DE ERROR
                            Log.Debug("Error " + FuncionSAP.IngresosEgresosFazones + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                        }
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.IngresosEgresosFazones + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.PesaNeto:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.PesaNeto + " id: " + comando.Id);
                        var request = new PesaNetoRequest
                        {
                            PesaNeto = Conversor.Convertir<PesaNetoTransmisionASap, PesaNeto>((PesaNetoTransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.PesaNeto + " id: " + comando.Id);
                        var respuesta = servicioSap.PesaNeto(request);

                        if (respuesta.PesaNetoResponse != null && respuesta.PesaNetoResponse.Mensajes.Any() && respuesta.PesaNetoResponse.Mensajes.All(x => x.MBLNR == "000"))
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto; //LLAMADA A SAP EXISTOSA
                            transmision.MensajeError = "";
                            servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                            {
                                InstanceId = transmision.InstanciaWorkflow,
                                DocumentoInternoSap = respuesta.PesaNetoResponse.Mensajes.FirstOrDefault().MBLNR,
                                NumeroDeDocumentoSap = respuesta.PesaNetoResponse.Mensajes.FirstOrDefault().XBLNR
                            });
                            Log.Debug("Correcto " + FuncionSAP.PesaNeto + " id: " + comando.Id);
                        }
                        else
                        {
                            transmision.MensajeError = respuesta.PesaNetoResponse.Mensajes.FirstOrDefault() != null ? respuesta.PesaNetoResponse.Mensajes.FirstOrDefault().TEXT : "Respuesta vacía";
                            Log.Debug("Error " + FuncionSAP.PesaNeto + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                        }
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.PesaNeto + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.FletesDobleTramo:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.FletesDobleTramo + " id: " + comando.Id);
                        var request = new FletesDobleTramoRequest
                        {
                            FletesDobleTramo = Conversor.Convertir<FletesDobleTramoTransmisionASap, FletesDobleTramo>((FletesDobleTramoTransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.FletesDobleTramo + " id: " + comando.Id);
                        var respuesta = servicioSap.FletesDobleTramo(request);

                        if (respuesta.FletesDobleTramoResponse != null && respuesta.FletesDobleTramoResponse.Resultado.MSGNR == "0")
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto; //LLAMADA A SAP EXISTOSA
                            transmision.MensajeError = "";
                            servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                            {
                                InstanceId = transmision.InstanciaWorkflow,
                                DocumentoInternoSap = respuesta.FletesDobleTramoResponse.Resultado.MBLNR,
                                NumeroDeDocumentoSap = respuesta.FletesDobleTramoResponse.Resultado.XBLNR
                            });
                            Log.Debug("Correcto " + FuncionSAP.FletesDobleTramo + " id: " + comando.Id);
                        }
                        else
                        {
                            transmision.MensajeError = respuesta.FletesDobleTramoResponse.Resultado != null ? respuesta.FletesDobleTramoResponse.Resultado.TEXT : "Respuesta vacía";
                            Log.Debug("Error " + FuncionSAP.FletesDobleTramo + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                        }
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.FletesDobleTramo + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.EgresoSinFleteFazones:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.EgresoSinFleteFazones + " id: " + comando.Id);
                        var fazonesRequest = new EgresoSinFleteFazonesRequest
                        {
                            EgresoSinFleteFazones = Conversor.Convertir<EgresoSinFleteFasonesTransmisionASap, EgresoSinFleteFazones>((EgresoSinFleteFasonesTransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.EgresoSinFleteFazones + " id: " + comando.Id);
                        var respuestaEgresos = servicioSap.EgresoSinFleteFazones(fazonesRequest);
                        if (respuestaEgresos.EgresoSinFleteFazonesResponse != null && !string.IsNullOrEmpty(respuestaEgresos.EgresoSinFleteFazonesResponse.Resultado.MBLNR))
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto; //LLAMADA A SAP EXISTOSA
                            transmision.MensajeError = "";
                            servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                            {
                                InstanceId = transmision.InstanciaWorkflow,
                                DocumentoInternoSap = respuestaEgresos.EgresoSinFleteFazonesResponse.Resultado.MBLNR,
                                NumeroDeDocumentoSap = respuestaEgresos.EgresoSinFleteFazonesResponse.Resultado.XBLNR
                            });
                            Log.Debug("Correcto " + FuncionSAP.EgresoSinFleteFazones + " id: " + comando.Id);
                        }
                        else if (respuestaEgresos.EgresoSinFleteFazonesResponse != null)
                        {
                            transmision.MensajeError = respuestaEgresos.EgresoSinFleteFazonesResponse.Resultado.TEXT; //COPIO MENSAJE DE ERROR
                            Log.Debug("Error " + FuncionSAP.EgresoSinFleteFazones + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                        }
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.EgresoSinFleteFazones + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.CartaPorteTransporteAutomotorRegistro:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.CartaPorteTransporteAutomotorRegistro + " id: " + comando.Id);
                        var fazonesRequest = new ParametrosRegistro
                        {
                            Item = Conversor.Convertir<CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto, CartaPorteTransporteAutomotorRegistro>((CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.CartaPorteTransporteAutomotorRegistro + " id: " + comando.Id);
                        var respuestaEgresos = servicioMonsanto.registrarCartaDePorte(new registrarCartaDePorte(fazonesRequest));
                        transmision.Estado = EstadoTransmisionASap.Correcto;
                        transmision.MensajeError = "";
                        var muestra = respuestaEgresos.respuesta.Item as MuestraRequerida;
                        if (muestra != null)
                        {
                            servicioComandos.Ejecutar(new ModificarCartaDePorteRegistradaServicioMonsanto
                            {
                                InstanceId = transmision.InstanciaWorkflow,
                                TipoAnalisis = muestra.tipoAnalisis.ToString(),
                                LaboratorioRazonSocial = muestra.laboratorio == null ? string.Empty : muestra.laboratorio.razonSocial,
                                LaboratorioCuit = muestra.laboratorio == null ? string.Empty : muestra.laboratorio.cuit.ToString(CultureInfo.InvariantCulture),
                            });
                        }
                        Log.Debug("Correcto " + FuncionSAP.CartaPorteTransporteAutomotorRegistro + " id: " + comando.Id);
                    }
                    catch (System.ServiceModel.FaultException<webServiceExceptionV2FaultDetailsBean> e)
                    {
                        transmision.MensajeError = e.Detail.errores != null && e.Detail.errores.Any() ? e.Detail.errores.First().descripcion.Truncate(250) + "(" + e.Detail.errores.First().codigo + ")" : e.Message;
                        Log.Error("Error " + FuncionSAP.CartaPorteTransporteAutomotorRegistro + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.CartaPorteTransporteAutomotorRegistro + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.CartaPorteVagonFerroviarioRegistro:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.CartaPorteVagonFerroviarioRegistro + " id: " + comando.Id);
                        var fazonesRequest = new ParametrosRegistro
                        {
                            Item = Conversor.Convertir<CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto, CartaPorteTransporteAutomotorRegistro>((CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.CartaPorteVagonFerroviarioRegistro + " id: " + comando.Id);
                        var respuestaEgresos = servicioMonsanto.registrarCartaDePorte(new registrarCartaDePorte(fazonesRequest));
                        transmision.Estado = EstadoTransmisionASap.Correcto;
                        transmision.MensajeError = "";
                        var muestra = respuestaEgresos.respuesta.Item as MuestraRequerida;
                        if (muestra != null)
                        {
                            servicioComandos.Ejecutar(new ModificarCartaDePorteRegistradaServicioMonsanto
                            {
                                InstanceId = transmision.InstanciaWorkflow,
                                TipoAnalisis = muestra.tipoAnalisis.ToString(),
                                LaboratorioRazonSocial = muestra.laboratorio == null ? string.Empty : muestra.laboratorio.razonSocial,
                                LaboratorioCuit = muestra.laboratorio == null ? string.Empty : muestra.laboratorio.cuit.ToString(CultureInfo.InvariantCulture),
                            });
                        }
                        Log.Debug("Correcto " + FuncionSAP.CartaPorteTransporteAutomotorRegistro + " id: " + comando.Id);
                    }
                    catch (System.ServiceModel.FaultException<webServiceExceptionV2FaultDetailsBean> e)
                    {
                        transmision.MensajeError = e.Detail.errores != null && e.Detail.errores.Any() ? e.Detail.errores.First().descripcion.Truncate(250) + "(" + e.Detail.errores.First().codigo + ")" : e.Message;
                        Log.Error("Error " + FuncionSAP.CartaPorteTransporteAutomotorRegistro + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.CartaPorteVagonFerroviarioRegistro + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro + " id: " + comando.Id);
                        var fazonesRequest = new registerSampleAndWeightRequest
                        {
                            Item = Conversor.Convertir<MuestreoPesajeTransporteAutomotorTransmisionAMonsanto, MuestreoPesajeTransporteAutomotor>((MuestreoPesajeTransporteAutomotorTransmisionAMonsanto)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro + " id: " + comando.Id);
                        var respuestaEgresos = servicioMonsanto.registrarMuestreoYPesaje(new registrarMuestreoYPesaje(fazonesRequest));
                        transmision.Estado = EstadoTransmisionASap.Correcto;
                        transmision.MensajeError = "";
                        Log.Debug("Correcto " + FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro + " id: " + comando.Id);
                    }
                    catch (System.ServiceModel.FaultException<webServiceExceptionV2FaultDetailsBean> e)
                    {
                        transmision.MensajeError = e.Detail.errores != null && e.Detail.errores.Any() ? e.Detail.errores.First().descripcion.Truncate(250) + "(" + e.Detail.errores.First().codigo + ")" : e.Message;
                        Log.Error("Error " + FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro + " id: " + comando.Id);
                        var fazonesRequest = new registerSampleAndWeightRequest
                        {
                            Item = Conversor.Convertir<MuestreoPesajeTransporteAutomotorTransmisionAMonsanto, MuestreoPesajeTransporteAutomotor>((MuestreoPesajeTransporteAutomotorTransmisionAMonsanto)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro + " id: " + comando.Id);
                        var respuestaEgresos = servicioMonsanto.registrarMuestreoYPesaje(new registrarMuestreoYPesaje(fazonesRequest));
                        transmision.Estado = EstadoTransmisionASap.Correcto;
                        transmision.MensajeError = "";
                        Log.Debug("Correcto " + FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro + " id: " + comando.Id);
                    }
                    catch (System.ServiceModel.FaultException<webServiceExceptionV2FaultDetailsBean> e)
                    {
                        transmision.MensajeError = e.Detail.errores != null && e.Detail.errores.Any() ? e.Detail.errores.First().descripcion.Truncate(250) + "(" + e.Detail.errores.First().codigo + ")" : e.Message;
                        Log.Error("Error " + FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.InformarCupo:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.InformarCupo + " id: " + comando.Id);
                        var cuposRequest = new Z_SDMF_Z2200NRequest
                        {
                            Z_SDMF_Z2200N = Conversor.Convertir<InformarCupoTransmisionASap, Z_SDMF_Z2200N>((InformarCupoTransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.InformarCupo + " id: " + comando.Id);
                        var respuesta = servicioSap.Z_SDMF_Z2200N(cuposRequest);
                        if (respuesta.Z_SDMF_Z2200NResponse != null &&
                            respuesta.Z_SDMF_Z2200NResponse.EX_RESULTADO != null &&
                            respuesta.Z_SDMF_Z2200NResponse.EX_RESULTADO.Any() &&
                            respuesta.Z_SDMF_Z2200NResponse.EX_RESULTADO.All(x => x.MSGNR == "000"))
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto;
                            transmision.MensajeError = "";
                            Log.Debug("Correcto " + FuncionSAP.InformarCupo + " id: " + comando.Id);
                        }
                        else if (respuesta.Z_SDMF_Z2200NResponse != null &&
                            respuesta.Z_SDMF_Z2200NResponse.EX_RESULTADO != null)
                        {
                            Log.Error("Respuesta con errores en la ejecución de la función InformarCupo");
                            transmision.MensajeError = respuesta.Z_SDMF_Z2200NResponse.EX_RESULTADO.FirstOrDefault() != null ?
                                respuesta.Z_SDMF_Z2200NResponse.EX_RESULTADO.First(x => x.MSGNR != "000").TEXT : "Respuesta Vacia";
                            Log.Debug("Error " + FuncionSAP.InformarCupo + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                            transmision.Estado = EstadoTransmisionASap.Error;
                        }

                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.InformarCupo + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
                case FuncionSAP.IngresosBodega:
                    try
                    {
                        var request = new IngresosBodegaRequest
                        {
                            IngresosBodega = Conversor.Convertir<IngresosBodegaTransmisionASap, IngresosBodega>((IngresosBodegaTransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.IngresosBodega + " id: " + comando.Id);
                        var respuesta = servicioSap.IngresosBodega(request);
                        if (respuesta.IngresosBodegaResponse != null && !string.IsNullOrEmpty(respuesta.IngresosBodegaResponse.EX_RESULTADO.MBLNR))
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto;
                            transmision.MensajeError = "";
                            servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                            {
                                InstanceId = transmision.InstanciaWorkflow,
                                DocumentoInternoSap = respuesta.IngresosBodegaResponse.EX_RESULTADO.MBLNR,
                                NumeroDeDocumentoSap = respuesta.IngresosBodegaResponse.EX_RESULTADO.XBLNR
                            });
                            Log.Debug("Correcto " + FuncionSAP.IngresosBodega + " id: " + comando.Id);
                        }
                        else if (respuesta.IngresosBodegaResponse != null)
                        {
                            Log.Error("Respuesta con errores en la ejecución de la función IngresosBodega");
                            transmision.MensajeError = respuesta.IngresosBodegaResponse.EX_RESULTADO.TEXT;
                            Log.Debug("Error " + FuncionSAP.IngresosBodega + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                            transmision.Estado = EstadoTransmisionASap.Error;
                        }

                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.IngresosBodega + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;

                case FuncionSAP.ZE7550:
                    try
                    {
                        Log.Debug("Obteniendo " + FuncionSAP.ZE7550 + " id: " + comando.Id);
                        var request = new Z_SDMF_RFC_ZE7550Request
                        {
                            Z_SDMF_RFC_ZE7550 = Conversor.Convertir<ZE7550TransmisionASap, Z_SDMF_RFC_ZE7550>((ZE7550TransmisionASap)transmision)
                        };
                        Log.Debug("Enviando " + FuncionSAP.ZE7550 + " id: " + comando.Id);
                        var respuestaZ1000 = servicioSap.Z_SDMF_RFC_ZE7550(request);

                        if (respuestaZ1000.Z_SDMF_RFC_ZE7550Response != null && respuestaZ1000.Z_SDMF_RFC_ZE7550Response.EX_RESULTADO.All(res => res.TIPO != "E"))
                        {
                            transmision.Estado = EstadoTransmisionASap.Correcto; //LLAMADA A SAP EXISTOSA
                            transmision.MensajeError = "";
                            Log.Debug("Correcto " + FuncionSAP.ZE7550 + " id: " + comando.Id);
                        }
                        else if (respuestaZ1000.Z_SDMF_RFC_ZE7550Response != null)
                        {
                            var mensaje = respuestaZ1000.Z_SDMF_RFC_ZE7550Response.EX_RESULTADO.First(res => res.TIPO == "E").TEXTO;
                            transmision.MensajeError = mensaje; //COPIO MENSAJE DE ERROR
                            Log.Debug("Error " + FuncionSAP.ZE7550 + " id: " + comando.Id + "Error: " + transmision.MensajeError);
                        }
                    }
                    catch (Exception ex)
                    {
                        transmision.MensajeError = ex.Message.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
                        Log.Error("Error " + FuncionSAP.ZE7550 + " id: " + comando.Id + "Error : " + transmision.MensajeError);
                    }
                    break;
            }

            resultado.Resultado = transmision.Estado == EstadoTransmisionASap.Error ? TipoAlerta.Error : TipoAlerta.Exito;
            transmision.Fecha = DateTime.Now;

            Repositorio.GuardarCambios();
            return resultado;
        }
    }
}
