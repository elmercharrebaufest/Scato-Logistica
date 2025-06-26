using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;
using static Molinos.Scato.Dominio.Constantes;
using Comando = Molinos.Scato.Dominio.Comandos.Comando;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarOrdenCargaFas : ProcesadorComando<ConsultarOrdenCargaFas>
    {
        private readonly ZSDWS_SCATO servicioSap;
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOperaciones servicioOperaciones;
        public ProcesadorConsultarOrdenCargaFas(IRepositorio repositorio, IConversor conversor, ILogger log, ZSDWS_SCATO servicioSap, IServicioRepositorio servicioRepositorio , IServicioComandos servicioComandos, IServicioOperaciones servicioOperaciones)
        : base(repositorio, conversor, log)
        {
            this.servicioSap = servicioSap;
            this.servicioRepositorio = servicioRepositorio;
            this.servicioComandos = servicioComandos;
            this.servicioOperaciones = servicioOperaciones;
        }

        public override Resultado Ejecutar(ConsultarOrdenCargaFas comando)
        {

            var resultado = new ResultadoConsultaOrdenCargaFas();

            try
            {
                Log.Info("Empieza el método FAS");
                resultado = ConsultaServicioSap(comando);
                if (!resultado.HayErrores && resultado.Orden.Count > 0)
                {
                    ConsultarCNRT(ref resultado);
                    ConsultarMoaOperaciones(ref resultado);
                }

            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo hacer la consulta de orden SAP");
                resultado.Errores.Add("SAP", "Error, el servicio de SAP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo hacer la consulta de CPE por destino");
                resultado.Errores.Add("SAP", Textos.Error_Generico);
            }
            return resultado;
        }

        private void GenerarRequestSap(ConsultarOrdenCargaFas comando, out ConsultaOrdenDeCargaRequest datosRequest) 
        {
            var centro = comando.Orden.Centro;
            var patente = comando.Orden.Patente;

            var consultaOrdenDeCarga = new ConsultaOrdenDeCarga { Centro = centro, Patente = patente };

            datosRequest = new ConsultaOrdenDeCargaRequest { ConsultaOrdenDeCarga = consultaOrdenDeCarga };

            Log.Info("Crea Request/ Request Centro " + datosRequest.ConsultaOrdenDeCarga.Centro + " Request Patente: " + datosRequest.ConsultaOrdenDeCarga.Patente);
        }

        private ResultadoConsultaOrdenCargaFas ConsultaServicioSap(ConsultarOrdenCargaFas comando) 
        {
            GenerarRequestSap(comando, out ConsultaOrdenDeCargaRequest datosRequest);

            Log.Info("Interceptando llamada a SAP: Consultar Orden");

            GenerarServicioSap(out ZSDWS_SCATO servicio);

            Log.Info("Empieza la llamada a SAP: Consultar Orden de Carga");
            var respuestaConsultaOrdenCarga = servicio.ConsultaOrdenDeCarga(datosRequest);
            Log.Info("Respuesta: " + respuestaConsultaOrdenCarga.ConsultaOrdenDeCargaResponse.Salida.ToXml());

            InterpretarResponseSap(respuestaConsultaOrdenCarga, comando.Orden.Workflow, out ResultadoConsultaOrdenCargaFas resultado);

            return resultado;
        }

        private void InterpretarResponseSap(ConsultaOrdenDeCargaResponse1 respuestaConsultaOrdenCarga, string workflow, out ResultadoConsultaOrdenCargaFas resultado)
        {
            resultado = new ResultadoConsultaOrdenCargaFas();
            if (respuestaConsultaOrdenCarga.ConsultaOrdenDeCargaResponse.Salida.Any())
            {
                Log.Info("Hay al menos un item en la respuesta");
                List<OrdenCargaFasDto> datosSap = new List<OrdenCargaFasDto>();

                var ordenCargaFas = respuestaConsultaOrdenCarga.ConsultaOrdenDeCargaResponse.Salida;
                int count = respuestaConsultaOrdenCarga.ConsultaOrdenDeCargaResponse.Salida.Count();

                for (int i = 0; i < count; i++)
                {
                    var numeroDocumentoChofer = ordenCargaFas[i].NRO_DOC_CHOFER;
                    if (ordenCargaFas[i].TIPO_DOC_CHOFER == TipoDocumentoChofer.Cuit && !string.IsNullOrEmpty(numeroDocumentoChofer))
                    {
                        if (numeroDocumentoChofer.Contains("-"))
                        {
                            var startPos = numeroDocumentoChofer.IndexOf("-");
                            var endPos = numeroDocumentoChofer.LastIndexOf("-");
                            numeroDocumentoChofer = numeroDocumentoChofer.Substring(startPos + 1, endPos - startPos - 1);
                        }
                        else if (numeroDocumentoChofer.Length >= 10)
                        {
                            numeroDocumentoChofer = numeroDocumentoChofer.Substring(2, 8);
                        }
                    }

                    var transportista = servicioRepositorio.ObtenerProveedorPorCuit(ConvertirCuil(ordenCargaFas[i].CUIT_TR), new TiposProveedor { PR = true });
                    var proveedor = servicioRepositorio.ObtenerProveedorPorCodigoSap(ordenCargaFas[i].KUNDE.TrimStart(new[] { '0' }));
                    var material = servicioRepositorio.ObtenerMaterialPorCodigoSap(ordenCargaFas[i].MATNR.TrimStart(new[] { '0' }));
                    var chofer = servicioRepositorio.ObtenerChoferPorNumeroDocumento(numeroDocumentoChofer);
                    var tipoComercial = servicioRepositorio.ObtenerTipoComercialPorCodigoSap(ordenCargaFas[i].TIPO_COMERCIAL);
                    var pagadorFlete = servicioRepositorio.ObtenerClientePorCodigoSap(ordenCargaFas[i].PAGADOR_FLETE);

                    if (material == null)
                    {
                        resultado.Error("Material", string.Format(Textos.OrdenCargaFAS_MaterialInexistente, ordenCargaFas[i].MATNR));
                        //return Json(new { datosSap = -1, error = string.Format(Textos.OrdenCargaFAS_MaterialInexistente, ordenCargaFas[i].MATNR) }, JsonRequestBehavior.AllowGet);
                    }
                    if (proveedor == null)
                    {
                        resultado.Error("Proveedor", string.Format(Textos.OrdenCargaFAS_ProveedorInexistente, ordenCargaFas[i].KUNDE));
                    }

                    if (!string.IsNullOrEmpty(ordenCargaFas[i].PAGADOR_FLETE) && pagadorFlete == null)
                    {
                        resultado.Error("PagadorFlete", string.Format(Textos.OrdenCargaFAS_PagadorFleteInexistente, ordenCargaFas[i].PAGADOR_FLETE));
                    }

                    if (transportista == null && proveedor == null)
                    {
                        resultado.Error("Transportista", string.Format(Textos.OrdenCargaFAS_TransportistaInexistente, ordenCargaFas[i].CUIT_TR));
                    }

                    if (workflow.Contains("Venta") && tipoComercial != null && !(tipoComercial.CodigoSap.Equals("998") || tipoComercial.CodigoSap.Equals("CYO")))
                    {
                        Log.Debug($"{ordenCargaFas[i].VBELN} no es Venta fas y tiene tipo comercial {ordenCargaFas[i].TIPO_COMERCIAL}");
                        continue;
                    }
                    if (workflow.Contains("Expo") && tipoComercial != null && !tipoComercial.CodigoSap.Equals("EFC"))
                    {
                        Log.Debug($"{ordenCargaFas[i].VBELN} no es Expo fas y tiene tipo comercial {ordenCargaFas[i].TIPO_COMERCIAL}");
                        continue;
                    }

                    var itemSap = new OrdenCargaFasDto
                    {
                        CuitTransporte = ConvertirCuil(ordenCargaFas[i].CUIT_TR),
                        MaterialId = material.Id,
                        MaterialDesc = material.Descripcion,
                        PatenteCamion = ordenCargaFas[i].PATEN,
                        TransportistaId = transportista?.Id ?? proveedor.Id,
                        TransportistaDesc = transportista?.RazonSocial ?? proveedor.RazonSocial,
                        PatenteAcoplado = ordenCargaFas[i].ACOPL,
                        NumeroOrden = ordenCargaFas[i].VBELN,
                        ValidaCompliance = !string.IsNullOrEmpty(ordenCargaFas[i].FLETEPROPIO),
                        Chofer = chofer,
                        TipoComercialDesc = tipoComercial?.Descripcion,
                        TipoComercialId = tipoComercial?.Id ?? 0,
                        DerivadoGranarioHabilitado = material.EsDerivadoGranario,
                        PlantaDGDestino = material.EsDerivadoGranario && !string.IsNullOrEmpty(ordenCargaFas[i].CODPLANTA) ? int.Parse(ordenCargaFas[i].CODPLANTA) : (int?)null,
                        OrdenDomicilioDestino = material.EsDerivadoGranario && !string.IsNullOrEmpty(ordenCargaFas[i].ORDENDOM) ? int.Parse(ordenCargaFas[i].ORDENDOM) : (int?)null,
                        PagadorFleteId = material.EsDerivadoGranario ? pagadorFlete?.Id : (int?)null,
                        PagadorFlete = material.EsDerivadoGranario ? pagadorFlete?.Descripcion : null,
                        PagadorFleteCuit = material.EsDerivadoGranario ? pagadorFlete?.Cuit : null,
                        Inhabilitado = workflow.Contains("Expo") ? false : !string.IsNullOrEmpty(ordenCargaFas[i].INHABILITADO),
                        TipoDomicilioDestino = material.EsDerivadoGranario && !string.IsNullOrEmpty(ordenCargaFas[i].TIPODOM) ? int.Parse(ordenCargaFas[i].TIPODOM) : (int?)null,
                    };

                    if (material.EsDerivadoGranario
                        && (!string.IsNullOrEmpty(ordenCargaFas[i].TIPO_REVENTA)
                        || (string.IsNullOrEmpty(ordenCargaFas[i].TIPO_REVENTA) && !string.IsNullOrEmpty(ordenCargaFas[i].CUIT_CTA_ORDEN))))
                    {
                        var cliente = servicioRepositorio.ListarClientesPorCuit(ConvertirCuil(ordenCargaFas[i].CUIT)).FirstOrDefault();
                        if (cliente == null)
                        {
                            resultado.Error("Cliente", string.Format(Textos.OrdenCargaFAS_ClienteInexistenteCUIT, Textos.Destino, ordenCargaFas[i].CUIT));
                        }
                        itemSap.ClienteCuit = cliente.Cuit;
                        itemSap.ClienteId = cliente.Id;
                        itemSap.ClienteDesc = cliente.Descripcion;
                    }
                    else
                    {
                        var cliente = servicioRepositorio.ObtenerClientePorCodigoSap(ordenCargaFas[i].KUNAG);
                        if (cliente == null)
                        {
                            resultado.Error("Cliente", string.Format(Textos.OrdenCargaFAS_ClienteInexistenteSAP, ordenCargaFas[i].KUNAG));
                        }
                        itemSap.ClienteCuit = cliente.Cuit;
                        itemSap.ClienteId = cliente.Id;
                        itemSap.ClienteDesc = cliente.Descripcion;
                    }

                    if (material.EsDerivadoGranario && ordenCargaFas[i].TIPO_REVENTA == SAP.TipoReventaComisionista && !string.IsNullOrEmpty(ordenCargaFas[i].CUIT_CTA_ORDEN))
                    {
                        var comisionista = servicioRepositorio.ObtenerClientePorCuit(ConvertirCuil(ordenCargaFas[i].CUIT_CTA_ORDEN));
                        var clienteProvisorio = servicioRepositorio.ObtenerClientePorCuit(ConvertirCuil(ordenCargaFas[i].CUIT));
                        itemSap.Comisionista = comisionista?.Descripcion;
                        itemSap.ComisionistaId = comisionista?.Id;
                    }
                    else if (material.EsDerivadoGranario && ordenCargaFas[i].TIPO_REVENTA == SAP.TipoReventaRemitente && !string.IsNullOrEmpty(ordenCargaFas[i].CUIT_CTA_ORDEN))
                    {
                        var remitente = servicioRepositorio.ObtenerClientePorCuit(ConvertirCuil(ordenCargaFas[i].CUIT_CTA_ORDEN));
                        itemSap.RemitenteCuit = remitente.Cuit;
                        itemSap.Remitente = remitente?.Descripcion;
                        itemSap.RemitenteId = remitente?.Id;
                    }

                    if (string.IsNullOrEmpty(ordenCargaFas[i].TIPO_REVENTA)
                        && material.EsDerivadoGranario)
                    {
                        if (!string.IsNullOrEmpty(ordenCargaFas[i].CUIT_CTA_ORDEN))
                        {
                            var clienteCodigo = string.Empty;

                            if (!string.IsNullOrEmpty(ordenCargaFas[i].CUIT_DESTINATARIO))
                                clienteCodigo = ordenCargaFas[i].CUIT_DESTINATARIO;
                            else
                                clienteCodigo = ordenCargaFas[i].CUIT_CTA_ORDEN;

                            var destinatario = servicioRepositorio.ListarClientesPorCuit(ConvertirCuil(clienteCodigo)).FirstOrDefault();

                            if (destinatario == null)
                            {
                                resultado.Error("Destinatario", string.Format(Textos.OrdenCargaFAS_ClienteInexistenteCUIT, Textos.Destinatario, clienteCodigo));
                            }

                            itemSap.DestinatarioId = destinatario.Id;
                            itemSap.DestinatarioDesc = destinatario.Descripcion;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(ordenCargaFas[i].CUIT_DESTINATARIO))
                            {
                                var destinatario = servicioRepositorio.ListarClientesPorCuit(ConvertirCuil(ordenCargaFas[i].CUIT_DESTINATARIO)).FirstOrDefault();

                                if (destinatario == null)
                                {
                                    resultado.Error("Destinatario", string.Format(Textos.OrdenCargaFAS_ClienteInexistenteCUIT, Textos.Destinatario, ordenCargaFas[i].CUIT_DESTINATARIO));
                                }
                                itemSap.DestinatarioCuit = destinatario.Cuit;
                                itemSap.DestinatarioId = destinatario.Id;
                                itemSap.DestinatarioDesc = destinatario.Descripcion;
                            }
                            else
                            {
                                itemSap.DestinatarioCuit = itemSap.ClienteCuit;
                                itemSap.DestinatarioId = itemSap.ClienteId;
                                itemSap.DestinatarioDesc = itemSap.ClienteDesc;
                            }
                        }
                    }
                    else if (material.EsDerivadoGranario)
                    {
                        if (!string.IsNullOrEmpty(ordenCargaFas[i].CUIT_DESTINATARIO))
                        {
                            var destinatario = servicioRepositorio.ListarClientesPorCuit(ConvertirCuil(ordenCargaFas[i].CUIT_DESTINATARIO)).FirstOrDefault();

                            if (destinatario == null)
                            {
                                resultado.Error("Destinatario", string.Format(Textos.OrdenCargaFAS_ClienteInexistenteCUIT, Textos.Destinatario, ordenCargaFas[i].CUIT_DESTINATARIO));
                            }

                            itemSap.DestinatarioId = destinatario.Id;
                            itemSap.DestinatarioDesc = destinatario.Descripcion;
                        }
                        else
                        {
                            itemSap.DestinatarioId = itemSap.ClienteId;
                            itemSap.DestinatarioDesc = itemSap.ClienteDesc;
                        }
                    }
                    
                    if (material.EsDerivadoGranario
                        && !string.IsNullOrEmpty(ordenCargaFas[i].CORRE)
                        && Regex.Replace(ordenCargaFas[i].CORRE, @"\s+", "") != "NOPOSEE")
                    {
                        var corredor = servicioRepositorio.ObtenerProveedorPorCodigoSap(ordenCargaFas[i].CORRE);
                        if (corredor == null)
                        {
                            resultado.Error("Corredor", string.Format(Textos.OrdenCargaFAS_ProveedorInexistenteCodigoSAP, Textos.Corredor, ordenCargaFas[i].CORRE));
                        }

                        itemSap.Corredor = corredor?.Descripcion;
                        itemSap.CorredorId = corredor?.Id;
                    }

                    if (material.EsDerivadoGranario && !string.IsNullOrEmpty(ordenCargaFas[i].PROV_INT_FLETE))
                    {
                        var intermediarioFlete = servicioRepositorio.ObtenerProveedorPorCodigoSap(ordenCargaFas[i].PROV_INT_FLETE.TrimStart(new[] { '0' }));
                        if (intermediarioFlete == null)
                        {
                            resultado.Error("IntermediarioFlete", string.Format(Textos.OrdenCargaFAS_ProveedorInexistenteCodigoSAP, Textos.CartaPorte_IntermediarioFlete, ordenCargaFas[i].PROV_INT_FLETE));
                        }
                        itemSap.IntermediarioFleteCuit = intermediarioFlete.Cuil;
                        itemSap.IntermediarioFleteId = intermediarioFlete.Id;
                        itemSap.IntermediarioFlete = intermediarioFlete.Descripcion;
                    }

                    datosSap.Add(itemSap);
                }

                if (datosSap.Count > 0)
                {
                    resultado.Orden = datosSap;
                }
                else
                {
                    resultado.Error("OrdenCargaFas", Textos.OrdenCargaFAS_Inexistente + "para " + workflow);
                }
            }
            else
            {
                resultado.Error("OrdenCargaFas", Textos.OrdenCargaFAS_Inexistente + "para " + workflow);
            }
        }

        private string ConvertirCuil(string cuil)
        {
            if (String.IsNullOrEmpty(cuil))
            {
                return "";
            }
            string validador1 = cuil.Substring(0, 2);
            string documento = cuil.Substring(2, 8);
            string validador2 = cuil.Substring(10, 1);
            return validador1 + "-" + documento + "-" + validador2;
        }

        private void ConsultarCNRT(ref ResultadoConsultaOrdenCargaFas resultado)
        {
            var ordenSap = resultado.Orden.First();
            string patenteResponse = ordenSap.PatenteCamion;
            string acopladoResponse = ordenSap.PatenteAcoplado;

            var comandoCNRT = GenerarComandoConsultarCNRT(ordenSap.PatenteCamion, ordenSap.PatenteAcoplado);
            var resultadoCategoriaCamion = servicioComandos.Ejecutar(comandoCNRT) as ResultadoEscalables;
            if (resultadoCategoriaCamion.HayErrores)
            {
                resultado.Error(nameof(OrdenCargaInternaDto.TipoVehiculo), resultadoCategoriaCamion.Errores.FirstOrDefault().Value);
                foreach (var item in resultado.Orden)
                {
                    item.TipoVehiculo = TipoVehiculo.Camión;
                }
            }
            else if (!resultadoCategoriaCamion.Categoria.HasValue)
            {
                resultado.Error(nameof(OrdenCargaInternaDto.TipoVehiculo), Textos.CategoriaEscalable_Nula);
                foreach (var item in resultado.Orden)
                {
                    item.TipoVehiculo = TipoVehiculo.Camión;
                }
            }
            else
                foreach (var item in resultado.Orden)
                {
                    item.TipoVehiculo = resultadoCategoriaCamion.Categoria.Value;
                }
        }

        private Comando GenerarComandoConsultarCNRT(string patente, string acoplado)
        {
            var configCNRTDummy = Repositorio.Obtener<Dominio.Entidades.ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.IngresarOrdenCargaInternaFason && x.Nombre == Constantes.ConfiguracionGeneral.CNRT.CNRTDummy && x.CentroId == null);
            if (configCNRTDummy != null && bool.TryParse(configCNRTDummy.Valor, out bool configCNRTDummyActive) && configCNRTDummyActive)
            {
                return new ConsultarEscalablesDummy
                {
                    Patente = patente,
                    Acoplado = acoplado,
                };
            }
            else
            {
                return new ConsultarEscalables
                {
                    Patente = patente,
                    Acoplado = acoplado,
                };
            }
        }
        private void ConsultarMoaOperaciones(ref ResultadoConsultaOrdenCargaFas resultado)
        {
            GenerarServicioOperaciones(out IServicioOperaciones servicio);

            foreach (var orden in resultado.Orden)
            {
                var materialCodigoSap = Repositorio.ObtenerProyeccion<Material, string>(x => x.Id == orden.MaterialId, x => x.CodigoSAP);
                var ordenesOperaciones = servicio.ObtenerOrdenesDeCargaFas(orden.PatenteCamion).Where(x => x.CodigoProducto == materialCodigoSap).ToList();

                if (ordenesOperaciones.Any())
                {
                    var ordenKmARecorrer = ordenesOperaciones.First().KmARecorrer;
                    if (!string.IsNullOrEmpty(ordenKmARecorrer) && int.TryParse(ordenKmARecorrer, out int kmARecorrer))
                    {
                        orden.KmARecorrer = kmARecorrer.ToString();
                        orden.TieneKmARecorrer = true;
                    }
                    else
                    {
                        orden.TieneKmARecorrer = false;
                    }
                }
                else
                {
                    orden.TieneKmARecorrer = false;
                }
            }
        }

        private void GenerarServicioOperaciones(out IServicioOperaciones servicio) 
        {
            var confiOperacionesDummy = servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.ServicioOperaciones, Constantes.ConfiguracionGeneral.Servicios.OperacionesDummy);
            bool usarMock = bool.Parse(confiOperacionesDummy.Valor);
            if (usarMock)
            {
                servicio = new ServicioSapMServicioOperacionesMock(Repositorio, Conversor); // Usa el mock en lugar del servicio real
                Log.Info("Usando servicio mock");
            }
            else
            {
                servicio = servicioOperaciones; // Usa el servicio real
            }
        }

        private void GenerarServicioSap(out ZSDWS_SCATO servicio)
        {
            Log.Info("Generando servicio SAP");
            this.servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.ServicioSap, Constantes.ConfiguracionGeneral.Servicios.SapDummy);
            var confiSapDummy = servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.ServicioSap, Constantes.ConfiguracionGeneral.Servicios.SapDummy);
            bool usarMock = bool.Parse(confiSapDummy.Valor);
            if (usarMock)
            {
                servicio = new ServicioSapMock(Repositorio , Conversor , Log); // Usa el mock en lugar del servicio real
                Log.Info("Usando servicio mock");
            }
            else
            {
                servicio = servicioSap; // Usa el servicio real
            }
        }
    }
}