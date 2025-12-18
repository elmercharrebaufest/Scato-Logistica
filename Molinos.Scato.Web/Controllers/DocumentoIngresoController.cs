using System;
using System.Linq;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Helpers;
using Ninject.Extensions.Logging;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Web.Controllers
{
    public abstract class DocumentoIngresoController : BaseController
    {
        protected readonly IServicioComandos servicioComandos;
        protected readonly ILogger log;
        protected ResultadoConsultarPagoTasaMunicipal ResultadoPagoTasaMunicipal;

        protected DocumentoIngresoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos) : base(servicio)
        {
            this.servicioComandos = servicioComandos;
            this.log = log;
        }

        protected bool SetearChofer(ChoferDto choferDto)
        {
            if (!ModelState.IsValid)
            {
                var Error = ModelState.Values.Where(c => c.Errors.Count > 0).ToList();
                return false;
            }
            log.Info("SetearChofer para el chofer con el CUIL: " + choferDto.Cuil);
            var chofer = servicio.BuscarChoferes(new ChoferFiltro { Cuil = choferDto.Cuil }).FirstOrDefault();
            if (chofer != null) //Chofer existente
            {
                log.Info("SetearChofer se actualizará el chofer con CUIL: " + choferDto.Cuil);
                var resultadoChofer = servicioComandos.Ejecutar(new ModificarChofer() { Dto = choferDto });
                //Verifico si hay errores
                if (resultadoChofer.HayErrores)
                {
                    log.Debug("SetearChofer - Errores: ");
                    resultadoChofer.Errores.ToList().ForEach(f =>
                    {
                        ModelState.AddModelError("Chofer." + f.Key, f.Value);
                        log.Debug(f.Value);
                    });
                    ModelState.AgregarErrores(resultadoChofer);
                    return false;
                }
            }
            else //ChoferNuevo
            {
                log.Info("SetearChofer se dará de alta el chofer con el CUIL: " + choferDto.Cuil);
                var resultadoChofer = servicioComandos.Ejecutar(new CrearChofer { Dto = choferDto });
                //Verifico si hay errores
                if (resultadoChofer.HayErrores)
                {
                    log.Debug("SetearChofer - Errores: ");
                    resultadoChofer.Errores.ToList().ForEach(f =>
                    {
                        ModelState.AddModelError("Chofer." + f.Key, f.Value);
                        log.Debug(f.Value);
                    });
                    ModelState.AgregarErrores(resultadoChofer);
                    return false;
                }
                choferDto.Id = (resultadoChofer as ResultadoCrear).Id;
            }
            return true;
        }

        protected bool SetearTransportista(ref int transportistaId, int tipoComercialId, bool esTransportista, bool esTransportistaTramo2 = false)
        {
            var tipoComercial = servicio.ObtenerTipoComercial(tipoComercialId);
            if (tipoComercial.TransportistaEsProveedor && (transportistaId == 0))
            {
                log.Debug("El transportista es obligatorio para el tipo comercial");
                if (esTransportistaTramo2)
                    ModelState.AddModelError("TransportistaTramo2", string.Format(Textos.Error_Requerido, Textos.Transportista_Segundo_Tramo));
                else
                    ModelState.AddModelError("Transportista", string.Format(Textos.Error_Requerido, Textos.Transportista));
                return false;
            }

            if (!esTransportista)
            {
                var proveedor = servicio.ObtenerProveedor(transportistaId);
                if (proveedor == null)
                {
                    if (!esTransportistaTramo2)
                        ModelState.AddModelError("Transportista", string.Format(Textos.Error_ProveedorInvalido));
                    else
                        ModelState.AddModelError("TransportistaTramo2", string.Format(Textos.Error_ProveedorInvalido));
                    transportistaId = 0;
                    return false;
                }

                var transportista = servicio.ObtenerTransportistaPorCuit(proveedor.Cuil);
                if (transportista != null) //Transportista Existente
                {
                    transportistaId = transportista.Id;
                }
                else //Creo el nuevo transportista
                {
                    try
                    {
                        var resultadoTransportista = servicioComandos.Ejecutar(new CrearTransportista
                        {
                            Dto = new TransportistaDto
                            {
                                Cuit = proveedor.Cuil,
                                Domicilio = proveedor.Domicilio,
                                LocalidadId = proveedor.LocalidadId,
                                ProvinciaId = proveedor.ProvinciaId,
                                RazonSocial = proveedor.RazonSocial
                            }
                        });
                        if (resultadoTransportista.HayErrores)
                        {
                            resultadoTransportista.Errores.ToList().ForEach(f => ModelState.AddModelError("Transportista", f.Value));
                            ModelState.AgregarErrores(resultadoTransportista);
                            return false;
                        }
                        transportistaId = (resultadoTransportista as ResultadoCrear).Id;
                    }
                    catch
                    {
                        ModelState.AddModelError("Transportista", string.Format(Textos.Error_ProveedorInvalido));
                    }
                }
            }
            return true;
        }

        protected void ConsultarPagoTasaMunicipal(int centroId, string patente, string acoplado, string nroCartaPorte, TipoVehiculo tipoVehiculo, string codigoEstablecimiento, bool esDemorado, int? materialId = null, Guid? workflowInstanceId = null)
        {
            log.Debug($"Validando tasa municipal para patente: {patente}, CTG/CP: {nroCartaPorte}, materialId:{materialId}");

            if (centroId == Constantes.Centro.IdSanLorenzo)
            {
                ResultadoPagoTasaMunicipal = servicioComandos.Ejecutar(new VerificarPagoTasaMunicipal
                {
                    Patente = patente,
                    Ctg = nroCartaPorte,
                    MaterialId = materialId,
                    PatenteAcoplado = acoplado,
                    CentroId = centroId,
                    TipoVehiculo = tipoVehiculo,
                    CodigoEstablecimiento = codigoEstablecimiento,
                    TipoOrigenDeValidacion = TipoOrigenDeValidacion.FormularioWorkflow,
                    Demorado = esDemorado,
                    InstanceId = workflowInstanceId
                }) as ResultadoConsultarPagoTasaMunicipal;

                if (ResultadoPagoTasaMunicipal != null)
                {
                    log.Debug($"Resultado Pago Tasa Municipal: {ResultadoPagoTasaMunicipal.MensajeAlerta}, Tipo Alerta: {ResultadoPagoTasaMunicipal.TipoAlerta}, Hay Error:{ResultadoPagoTasaMunicipal.HayErrores}");

                    if (!string.IsNullOrEmpty(ResultadoPagoTasaMunicipal.MensajeAlerta) && !ResultadoPagoTasaMunicipal.HayErrores)
                    {
                        TempData["AlertaTasaMunicipal"] = ResultadoPagoTasaMunicipal.MensajeAlerta;
                        TempData["TipoAlertaTasaMunicipal"] = ResultadoPagoTasaMunicipal.TipoAlerta;
                    }
                }
                if (!esDemorado && ResultadoPagoTasaMunicipal != null && !ResultadoPagoTasaMunicipal.EjecutaWorkFlow)
                {
                    ModelState.AddModelError("ErrorTasaMunicipal", "");
                }
            }
        }

        protected void ActualizarTasaMunicipal(Guid instanciaWorkflowId, int? idPago, int idDiferenciaDePago = 0)
        {
            if (idPago != null)
            {
                servicioComandos.Ejecutar(new ModificarComoUsadoPagosTasaMunicipal
                {
                    InstanceId = instanciaWorkflowId,
                    PagoId = idPago.Value,
                    DiferenciaPagoId = idDiferenciaDePago
                });
            }
        }

        protected void ActualizarExcepcionPorPatente(Guid workflowInstanceId, int idExcepcion)
        {
            try
            {
                servicioComandos.Ejecutar(new ModificarExcepcionPagoTasaMunicipal
                {
                    Id = idExcepcion,
                    WorkflowInstanceId = workflowInstanceId, 
                    Usuario = "SCATO"
                });
            }
            catch (Exception e)
            {
                log.Error("Error al modificar la excepcion - {0}", e.Message);
            }
        }

        protected void MarcarRecorridoComoContingencia(Guid instanciaWorkflowId)
        {
            try
            {
                if (instanciaWorkflowId != Guid.Empty)
                {
                    servicioComandos.Ejecutar(new ModificarRecorridoPorContingenciaPay
                    {
                        InstanceId = instanciaWorkflowId
                    });
                }
            }
            catch (Exception e)
            {
                log.Error("Error al modificar la excepcion - {0}", e.Message);
            }
        }

        protected void InformarPagoTasaMunicipal(Guid instanciaWorkflowId)
        {
            try
            {
                servicioComandos.Ejecutar(new ModificarInformadoPagosTasaMunicipal
                {
                    InstanceId = instanciaWorkflowId
                });
            }
            catch (Exception e)
            {
                log.Error("Error al informar Pago Tasa Municipal - {0}", e.Message);
            }
        }

        protected void CrearRecorridoTasaMunicipal(Guid instanceId, string motivoExcepcion, bool tieneExcepcion)
        {
            try
            {
                var idRecorrido = servicio.ObtenerRecorridoIdPorGuid(instanceId);
                servicioComandos.Ejecutar(new CrearModificarRecorridoTasaMunicipal
                {
                    Id = idRecorrido,
                    MotivoExcepcion = motivoExcepcion,
                    TieneExcepcion = tieneExcepcion
                });
            }
            catch (Exception e)
            {
                log.Error("Error al crear Recorrido Tasa Municipal - {0}", e.Message);
            }
        }

        protected virtual bool EjecutarAccionesDePagoTasaMunicipalPosteriorALaCreacionDeWorkflow(int centroId, Guid workflowInstanceId)
        {
            bool resultado = true;

            if (centroId == Constantes.Centro.IdSanLorenzo)
            {
                try
                {
                    if (this.ResultadoPagoTasaMunicipal != null)
                    {
                        if (ResultadoPagoTasaMunicipal.IdPago.HasValue && ResultadoPagoTasaMunicipal.IdPago > 0)
                            ActualizarTasaMunicipal(workflowInstanceId, ResultadoPagoTasaMunicipal.IdPago.Value, ResultadoPagoTasaMunicipal.IdDiferenciaDePago ?? 0);

                        if (ResultadoPagoTasaMunicipal.IdExcepcion > 0)
                        {
                            InformarPagoTasaMunicipal(workflowInstanceId);
                            ActualizarExcepcionPorPatente(workflowInstanceId, ResultadoPagoTasaMunicipal.IdExcepcion);
                        }

                        CrearRecorridoTasaMunicipal(workflowInstanceId, ResultadoPagoTasaMunicipal.MotivoExcepcion, ResultadoPagoTasaMunicipal.TieneExcepcion);
                    }

                    if (servicio.TieneContingenciaPorTipo(Constantes.Contingencia.PayCaido))
                        MarcarRecorridoComoContingencia(workflowInstanceId);
                }
                catch (Exception ex)
                {
                    resultado = false;
                    this.log.Error(ex, "Ocurrió un error al realizar acciones de pago de tasa municipal posteriores a la creación del workflow.");
                    throw;
                }
            }

            return resultado;
        }
    }
}