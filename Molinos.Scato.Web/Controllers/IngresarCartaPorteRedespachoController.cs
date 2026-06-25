using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadIngresarCartaPorteRedespacho)]
    public class IngresarCartaPorteRedespachoController : CargarCartaPorteController
    {
        public IngresarCartaPorteRedespachoController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICargarCartaPorteService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows, IFirmaProvider configuracion, ZSDWS_SCATO servicioSap, IServicioOrquestador servicioOrquestador)
            : base(log, servicio, factory, servicioComandos, workflows, configuracion, servicioSap, servicioOrquestador)
        {
        }

        [DatosUsuario]
        public override ActionResult Index(string workflow, DatosUsuario datosUsuario, string destinatarioCodigoSap = "", string titularCodigoSap = "", string centroDestino = "", string rtteComercial = "", int cargaDeCupoId = 0)
        {
            log.Info($"IngresarCartaPorteRedespachoController Index: workflow={workflow}, datosUsuario={datosUsuario}, destinatarioCodigoSap={destinatarioCodigoSap}, titularCodigoSap={titularCodigoSap}, centroDestino={centroDestino}, rtteComercial={rtteComercial}, cargaDeCupoId={cargaDeCupoId}");
            var firmaCodigoSap = configuracion.ObtenerFirmaSinLogo().CodigoSAP;
            return base.Index(workflow, datosUsuario, string.IsNullOrEmpty(destinatarioCodigoSap) ? firmaCodigoSap : destinatarioCodigoSap, string.IsNullOrEmpty(titularCodigoSap) ? firmaCodigoSap : titularCodigoSap, centroDestino, rtteComercial, cargaDeCupoId);
        }

        protected override void SetearVista(WorkflowDto workflow, int centroId)
        {
            base.SetearVista(workflow, centroId);
            ViewBag.DeshabilitarTitular = false;
            ViewBag.DeshabilitarDestinatario = true;
            ViewBag.DeshabilitarEntregador = false;
            ViewBag.AceptaPendiente = true;
        }

        [DatosUsuario]
        public JsonResult ObtenerCartaPorteRedespacho(string numero, string workflow, bool esIngreso, int tipoVehiculo, bool cpe, bool consultactg, DatosUsuario datosUsuario)
        {
            try
            {
                if (!esIngreso)
                {
                    log.Debug("Obteniendo carta de porte nro {0} workflow {1}", numero, workflow);
                    return ObtenerCartaPorte(numero, workflow, datosUsuario);
                }
                log.Debug("Obteniendo carta de porte redespacho nro {0} workflow {1}", numero, workflow);
                var cartaPorteResponse = servicio.ObtenerCartaPorteRedespachoPorNumero(numero, datosUsuario.CentroId, workflow, tipoVehiculo, cpe, consultactg);
                log.Debug($"ObtenerCartaPorteRedespacho: {cartaPorteResponse.CartaPorte}");
                if (cartaPorteResponse.CartaPorte != null)
                {
                    log.Debug("Se Obtuvo la carta de porte redespacho nro {0} workflow {1}", numero, workflow);
                    var respuestaAFIP = servicioComandos.Ejecutar(new ConsultarAFIP
                    {
                        CentroId = datosUsuario.CentroId,
                        TipoVehiculoId = tipoVehiculo,
                        NumeroCartaOrden = numero,
                    }) as ResultadoConsultarAFIP;

                    if (!respuestaAFIP.HayErrores && respuestaAFIP?.TarifaReferencia != null)
                    {
                        log.Debug("Se uso la Tarifa Referencia de AFIP {0} para el Numero Carta Porte {1}", respuestaAFIP.TarifaReferencia, numero);
                        cartaPorteResponse.CartaPorte.TarifaReferencia = (decimal)respuestaAFIP.TarifaReferencia;
                    }
                    else if (respuestaAFIP.HayErrores)
                    {
                        foreach (var item in respuestaAFIP.Errores)
                        {
                            log.Error(item.Key + " - " + item.Value);
                        }
                    }
                }
                else
                {
                    long numeroCtg = long.TryParse(numero, out var ctg) ? ctg : 0;
                    var resultadoCartaPorteElectronica = servicioComandos.Ejecutar(new ConsultarCPDigital { NroCtg = numeroCtg, Usuario = datosUsuario.NombreUsuario, CentroId = datosUsuario.CentroId, TipoVehiculo = tipoVehiculo, ConsultaFerroviarioPorCtg = consultactg }) as ResultadoCartaPorteElectronica;
                    if(resultadoCartaPorteElectronica != null && resultadoCartaPorteElectronica.Cpe != null)
                    {
                        log.Debug("Se obtuvo la carta de porte digital para el CTG {0}", numero);
                        cartaPorteResponse.CartaPorte = resultadoCartaPorteElectronica.Cpe;
                        cartaPorteResponse.EsRedespacho = true;
                        cartaPorteResponse.Error = string.Empty;
                        cartaPorteResponse.CodigoDeError = 0;
                    }
                    else
                    {
                        log.Debug("No se encontró carta de porte digital para el CTG {0}", numero);
                    }
                    log.Info($"Response: {cartaPorteResponse.EsRedespacho}, {cartaPorteResponse.Error} , {cartaPorteResponse.CodigoDeError}");
                }
                return Json(cartaPorteResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener la carta de porte nro {0}", numero);
                throw;
            }
        }

        protected override CartaPorteValidaResponseDto ValidarNumeroCartaPorte(CartaPorteDto orden, int centroId, string workflowCodigo)
        {
            return servicio.NumeroCartaPorteValidoRedespacho(orden.NroCartaPorte, centroId, workflowCodigo);
        }

        protected override bool Validar(CartaPorteDto orden, DatosUsuario usuario)
        {
            if (orden.TipoDeWorkflow == TipoDeWorkflow.Egreso && !servicio.ProcedenciaYCodigoValido(usuario.CentroId, orden.CodEstab, orden.ProcedenciaId))
            {
                ModelState.AddModelError("", Textos.Error_ProcedenciaInvalida);
                return false;
            }

            var codigoSapMolinos = configuracion.ObtenerFirmaSinLogo().CodigoSAP;
            var codigoSapMRP = ConfigurationManager.AppSettings["CodigoSapMRP"];
            var codigoSapTitular = servicio.ObtenerProveedor(orden.TitularCartaPorteId)?.CodigoSap;
            var codigoSapRemitenteComercial = servicio.ObtenerProveedor(orden.RtteComercialId)?.CodigoSap;
            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);
            orden.DestinoCodigoSap = servicio.ObteneCodigoSapPorCentroId(orden.DestinoId);
            orden.DestinatarioCodigoSap = servicio.ObtenerProveedor(orden.DestinatarioId)?.CodigoSap;
               
            var escenario = WorkflowCartaPorteHelper.ObtenerEscenario(
                codigoSapTitular,
                codigoSapRemitenteComercial,
                orden.DestinoCodigoSap,
                orden.DestinatarioCodigoSap,
                codigoSapMolinos,
                codigoSapMRP,
                orden.CodEstab,
                orden.RtteComercialVentaSecundarioCuil);

            if (escenario != EscenarioWorkflowCartaPorte.Redespacho)
            {
                ModelState.AddModelError("", Textos.Error_CCPPRedespacho);
                return false;
            }
            if (otroRecorridoDelChofer != null)
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                return false;
            }

            return true;
        }

        [DatosUsuario]
        protected override ControlRecorridoDto GenerarControlRecorrido(DatosUsuario usuario)
        {
            return new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresarCartaPorteRedespacho,
                ActividadXaml = "IngresarCartaPorteRedespacho",
                PuestoDeTrabajoId = usuario.PuestoDeTrabajoId,
                NombreUsuario = usuario.NombreUsuario
            };
        }
    }
}