using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Consultas;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.CargaDeCupo)]
    public class CargaDeCupoController : BaseController
    {
        private ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IListaDeWorkflows workflows;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IConfiguracionProvider configuracion;
        private readonly IFirmaProvider firma;
        private readonly IServicioActividadFactory<ICargarCartaPorteService> factory;
        private readonly IServicioActividadFactory<IIngresarOrdenCargaInternaService> factoryNoProductivo;
        private readonly IServicioActividadFactory<IIngresarOrdenCargaInternaFasonService> factoryFason;
        private readonly IServicioActividadFactory<IIngresarOrdenCargaFasService> factoryFas;

        // Claves de intervinientes que deben tratarse como errores no-bloqueantes (solo advertencia)
        private static readonly string[] ClavesIntervinientesNoBloqueantes = new[]
        {
            nameof(CartaPorteDto.Transportista),
            Textos.CartaPorte_RtteComercial,
            Textos.CartaPorte_TitularCartaPorte,
            Textos.CartaPorte_Intermediario,
            Textos.CartaPorte_Destinatario,
            Textos.CartaPorte_Entregador,
            Textos.CartaPorte_AgenteCompras,
            Textos.CartaPorte_CorredorVendedor,
            Textos.CartaPorte_IntermediarioFlete,
            Textos.Corredor_primario,
            Textos.Rtte_comercial_venta_secundaria_2,
            Textos.CartaPorte_RtteComercialProductor,
            Textos.CartaPorte_RtteComercialVentaSecundario,
            Textos.CartaPorte_Transportista_Pagador_Flete
        };

        public CargaDeCupoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos,
            IListaDeWorkflows workflows, ZSDWS_SCATO servicioSap, IServicioOrquestador servicioOrquestador,
            IConfiguracionProvider configuracion, IFirmaProvider firma,
            IServicioActividadFactory<ICargarCartaPorteService> factory,
            IServicioActividadFactory<IIngresarOrdenCargaInternaService> factoryNoProductivo,
            IServicioActividadFactory<IIngresarOrdenCargaInternaFasonService> factoryFason,
            IServicioActividadFactory<IIngresarOrdenCargaFasService> factoryFas)
            : base(servicio)
        {
            this.servicioComandos = servicioComandos;
            this.log = log;
            this.workflows = workflows;
            this.servicioSap = servicioSap;
            this.servicioOrquestador = servicioOrquestador;
            this.firma = firma;
            this.factory = factory;
            this.configuracion = configuracion;
            this.factoryNoProductivo = factoryNoProductivo;
            this.factoryFason = factoryFason;
            this.factoryFas = factoryFas;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            SetearVista(datosUsuario);
            var model = new CargaDeCupoDto { CPE = true };
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public JsonResult Index(CargaDeCupoDto model, bool AvanceCpe, DatosUsuario datosUsuario)
        {
            log.Debug("CartaDePorte {0}, Tarjeta {1}, Centro {2}, Patente {3}", model.NumeroCartaPorte, model.Numero, datosUsuario.CentroId, model.Patente);
            var response = new CargaDeCupoResponseDto();

            if (!ModelState.IsValid)
            {
                response.ValidationErrors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.First().ErrorMessage ??
                               kvp.Value.Errors.First().Exception?.Message ??
                               "error"
                    );
                response.Success = false;
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            if (model.CircuitoNoGranos)
                return IndexNoGranos(model, datosUsuario);

            if (servicio.EsTarjetaBloqueada(model.Numero, datosUsuario.CentroId))
            {
                log.Debug("ERROR 2 de cupo {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                response.ValidationErrors.Add("", Textos.AsignacionTarjetaDeAcceso_TarjetaBloqueada);
                response.Success = false;
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            if (!servicio.EsTarjetaEnRangoValido(model.Numero, datosUsuario.CentroId))
            {
                log.Debug("ERROR 3 de cupo {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                response.ValidationErrors.Add("", Textos.AsignacionTarjetaDeAcceso_TarjetaSinRango);
                response.Success = false;
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            var instanciaWorkflow = servicio.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(model.Numero, datosUsuario.CentroId);
            if (workflows.VerificarExistenciaDeWorkflowPorGuid(instanciaWorkflow))
            {
                log.Debug("ERROR 4 de cupo {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                response.ValidationErrors.Add("", Textos.ImpresionTarjetaDeAcceso_EnUso);
                response.Success = false;
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            var validarTarjetaEnUsoPendienteSinRecorrido = ConfigurationManager.AppSettings["ValidarTarjetaEnUsoEtapaPendiente"];
            if (!string.IsNullOrEmpty(validarTarjetaEnUsoPendienteSinRecorrido) && validarTarjetaEnUsoPendienteSinRecorrido == "1")
            {
                var intanciaWorkflow = workflows.ObtenerWorkflowPendientePorNumeroTarjetaAcceso(model?.Numero, datosUsuario?.CentroId);
                if (!(intanciaWorkflow is null))
                {
                    response.ValidationErrors.Add("", string.Format(Textos.TarjetaDeAcceso_EnUso_Pendiente, intanciaWorkflow.Patente));
                    response.Success = false;
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }

            var tipoVariedadCodigo = Constantes.TipoVariedadMaterial.Estandar;
            if (model.Cupo != Constantes.ValoresPorDefecto.CupoGenerico)
            {
                var configuracionOmitirDataAgro = servicio.ObtenerConfiguracionGeneral(
                    Constantes.ConfiguracionGeneral.Pantalla.CargaDeCupo,
                    Constantes.ConfiguracionGeneral.CargaDeCupo.OmitirValidacionDataAgroVisec);
                var omitirValidacionDataAgroVisec = !string.IsNullOrEmpty(configuracionOmitirDataAgro?.Valor)
                    && bool.TryParse(configuracionOmitirDataAgro.Valor, out bool omitir)
                    && omitir;

                if (!omitirValidacionDataAgroVisec)
                {
                    var resultadoConsultaDataAgroVisec = servicioComandos.Ejecutar(new ConsultarDataAgroVisec
                    {
                        Cupo = model.Cupo,
                    }) as ResultadoConsultarDataAgroVisec;
                    if (resultadoConsultaDataAgroVisec == null || resultadoConsultaDataAgroVisec.HayErrores)
                    {
                        response.ValidationErrors.Add("", "Ocurrió un error al consultar cupo en Data Agro");
                        response.Success = false;
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    tipoVariedadCodigo = resultadoConsultaDataAgroVisec.CodigoVariedad;

                    if (servicio.TieneContingenciaPorTipo(Constantes.Contingencia.VisecCaido))
                    {
                        if (tipoVariedadCodigo == Constantes.TipoVariedadMaterial.EPAyEUDR
                        || tipoVariedadCodigo == Constantes.TipoVariedadMaterial.EUDR
                        || tipoVariedadCodigo == Constantes.TipoVariedadMaterial.EPA)
                        {
                            response.ValidationErrors.Add("", "Contingencia Visec Activada");
                            response.Success = false;
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (tipoVariedadCodigo == Constantes.TipoVariedadMaterial.EUDR || tipoVariedadCodigo == Constantes.TipoVariedadMaterial.EPAyEUDR)
                    {
                        var resultadoValidarStock = ValidarVisec(model.Cosecha, model.MaterialId.GetValueOrDefault(), model.PesoNetoOrigen, model.CodEstab, model.CodigoRENSPA);
                        if (resultadoValidarStock.HayErrores)
                        {
                            response.ValidationErrors.Add("", resultadoValidarStock.Errores.FirstOrDefault().Value);
                            response.Success = false;
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                    }
		        }
            }

            var resultadoConsultarTasa = new ResultadoConsultarPagoTasaMunicipal();
            model.ImagenCartaPorte = GenerarImagenCartaPorte(model);
            model.Fecha = DateTime.Now;
            model.CentroId = datosUsuario.CentroId;
            model.CentroCodigoSap = datosUsuario.CentroCodigoSap;
            model.Patente = (model.Patente ?? string.Empty).ToUpper();
            var resultado = servicioComandos.Ejecutar(new CrearCargaDeCupo { Dto = model, EsGarita = true }) as ResultadoCrear;
            if (resultado == null)
            {
                log.Error("CrearCargaDeCupo retornó resultado nulo o tipo inesperado.");
                response.ValidationErrors.Add("", Textos.Error_Generico);
                response.Success = false;
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            if (resultado.HayErrores)
            {
                log.Debug("ERROR 5 de cupo {0}, CP {1}: {2}", model.Cupo, model.NumeroCartaPorte, resultado.Errores.First().Value);
                log.Debug($"ERROR: {resultado.Errores.First().Key}");
                response.ValidationErrors.Add("", resultado.Errores.First().Value);
                response.Success = false;
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else
            {
                response.Success = true;
                servicioComandos.Ejecutar(
                    new RegistrarMarcaDeTiempo 
                    { 
                        Tipo = TipoSensorMarcaTiempo.FinPorGaritaIngreso, 
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NumeroDeTarjeta = model.Numero,
                    }
                );
            }

            Task.Run(() =>
            {
                try { servicioComandos.Ejecutar(new ValidarAccesoStopBandasHorarias { CTG = model.CTG, Patente = model.Patente }); }
                catch (Exception ex) { log.Error(ex, "Error al validar acceso stop bandas horarias para CTG {0}", model.CTG); }
            });

            model.FotoRutaDestino = resultado.Mensaje;
            model.FotoRutaSustentable = resultado.PathSustentable;

            if (model.CentroId == Constantes.Centro.IdSanLorenzo)
            {
                resultadoConsultarTasa = servicioComandos.Ejecutar(new VerificarPagoTasaMunicipal
                {
                    Patente = model.Patente,
                    Ctg = model.CPE ? model.CTG : model.NumeroCartaPorte,
                    MaterialId = model.MaterialId.GetValueOrDefault(),
                    PatenteAcoplado = model.PatenteAcoplado,
                    CentroId = model.CentroId,
                    EsNoGranos = model.CircuitoNoGranos,
                    CodigoEstablecimiento = model.CodEstab,
                    TipoOrigenDeValidacion = TipoOrigenDeValidacion.CargaDeCupo
                }) as ResultadoConsultarPagoTasaMunicipal;
            }

            if (!model.NoAsignaCalleEnGaritaEntrada)
            {
                log.Debug("Asignar Calle: Resultado Id= {0}, Patente: {1}, MaterialId: {2}", resultado.Id, model.Patente, model.MaterialId);
                var turnoActivo = InformarArribo(model.CPE ? model.CTG : model.NumeroCartaPorte, datosUsuario.CentroId, model.Patente, model.MaterialId.GetValueOrDefault());
                var codigoBarrera = servicio.ObtenerDispositivoBarreraEntrada(model.PuestoDeTrabajoId);

                if (PermitirAsignarCalleGrano(model.TitularCartaPorteCodigoSap, model.CodEstab, model.RtteComercialCodigoSap, model.RtteComercialVentaSecundariaCuit))
                    AsignarCalle(resultado.Id, turnoActivo, datosUsuario.NombrePc, model, resultadoConsultarTasa, response);

                log.Info($"Ejecutando Apertura Barrera Garita con CodigoBarrera : {codigoBarrera} y Patente : {model.Patente}");
                if (resultadoConsultarTasa.SeLevantaBarrera)
                    AperturaDeBarrera(codigoBarrera);
            }

            if (model.ImprimeTarjetaDeAcceso)
                ImprimirTarjetaDeAcceso(model, resultado.Id, response);

            if (response.Success)
            {
                if (AvanceCpe && EsCupoValidoParaAvanceAutomatico(model, tipoVariedadCodigo)
                    && !resultadoConsultarTasa.HayErrores
                    && resultadoConsultarTasa.EjecutaWorkFlow)
                {
                    servicioComandos.Ejecutar(new SetearProgresoCargaDeCupo() { Id = resultado.Id, EnProgresoAutomatico = true });
                    CargarCartaPorte(resultado.Id, datosUsuario, model.ImagenCartaPorte, resultadoConsultarTasa, response, tipoVariedadCodigo);
                    servicioComandos.Ejecutar(new SetearProgresoCargaDeCupo() { Id = resultado.Id, EnProgresoAutomatico = false });
                }
                ProcesarResultadoTasaMunicipal(resultadoConsultarTasa.TipoAlerta, resultadoConsultarTasa.MensajeAlerta, response);
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private bool InformarArribo(string cartaPorte, int centroId, string patente, int materialId)
        {
            var responseCircular = servicioComandos.Ejecutar(new InformarArriboCircular
            {
                CentroId = centroId,
                CartaPorte = cartaPorte,
                Patente = patente,
                MaterialId = materialId
            });

            var resp = responseCircular as ResultadoCircular;

            if (resp.HayErrores)
            {
                log.Error(resp.Errores.Values.First());
            }
            return resp.TurnoActivo;
        }

        private void ImprimirTarjetaDeAcceso(CargaDeCupoDto model, int cargaDeCupoId, CargaDeCupoResponseDto response)
        {
            var resultadoImpresion = servicioComandos.Ejecutar(new ImprimirTarjetaDeAcceso
            {
                Dto = new ImpTarjetaDeAccesoDto
                {
                    Codigo = "ImpresionTarjetaDeAcceso",
                    Numero = model.Numero,
                    Fecha = DateTime.Now.Formatted(),
                    CentroId = model.CentroId,
                    PuestoDeTrabajoId = model.PuestoDeTrabajoId
                },
                OrigenImpresion = "CargaDeCupoController"
            });
            if (resultadoImpresion.HayErrores)
            {
                servicioComandos.Ejecutar(new EliminarCargaDeCupo { Id = cargaDeCupoId });
                response.ValidationErrors.Add("Imp", Textos.ErrorImpresionTarjetaDeAcceso);
                response.Success = false;
            }
        }

        private void AsignarCalle(int cargaDeCupoId, bool turnoActivo, string nombrePc, CargaDeCupoDto model, ResultadoConsultarPagoTasaMunicipal resultadoTazaMunicipal, CargaDeCupoResponseDto response)
        {
            var resultado = servicioComandos.Ejecutar(new CrearCallePorRecorrido
            {
                TipoCalle = TipoCalle.PreCalado,
                CargaDeCupoId = cargaDeCupoId,
                TurnoActivo = turnoActivo,
                CentroId = model.CentroId
            }) as ResultadoCrearCalle;
            if (resultado.HayErrores)
            {
                servicioComandos.Ejecutar(new EliminarCargaDeCupo { Id = cargaDeCupoId });
                response.ValidationErrors.Add("warning", resultado.Errores.First().Value);
                response.Success = false;
            }

            var fila = servicio.ObtenerCalleNombre(resultado.Id);
            response.Message = resultado.Disponibilidad <= 5 && resultado.Disponibilidad > 0
                ? $"Carga Exitosa, se asignó {fila}, ESPACIO DISPONIBLE: {resultado.Disponibilidad} camiones"
                : $"Carga Exitosa, se asignó {fila}";

            servicioComandos.Ejecutar(new EnviarMensajeCamioneroCircular
            {
                CartaPorte = model.CPE ? model.CTG : model.NumeroCartaPorte,
                Mensaje = string.Format("Por favor avanzar, ubicarse en la \"{0}\" y espere a ser llamado para calado", fila),
                SePuedeDesactivar = true
            });

            log.Debug($"Fila asignada {fila} por el puestoId: {nombrePc}");
            MostrarPorCartel(nombrePc, fila, model.CentroId, model.Patente, resultadoTazaMunicipal.MensajeAlerta, resultadoTazaMunicipal.TipoAlerta);
        }

        private void MostrarPorCartel(string nombrePc, string mensaje, int centroId, string patente, string mensajeTasaMunicipal, TipoAlerta tipoAlerta)
        {
            var codigoMensaje = tipoAlerta == TipoAlerta.Exito
                    ? CodigoMensajeCartelLed.GaritaIngresoConTasaMunicipalExito
                    : CodigoMensajeCartelLed.GaritaIngresoConTasaMunicipalError;

            var puestoDeTrabajo = servicio.ObtenerPuestoDeTrabajoPorNombrePc(nombrePc, centroId);
            var mensajesCartel = servicio.ListarMensajesCartelLed(codigoMensaje);
            var mensajes = mensajesCartel
                .OrderBy(s => s.Orden)
                .Select(s =>
                    new EnviarMensajeCartelLed
                    {
                        Mensaje = string.Format(s.Mensaje,
                                                mensaje,                       // {0}
                                                patente,                       // {1}
                                                tipoAlerta == TipoAlerta.Exito ? mensajeTasaMunicipal : "",    // {2} - solo se usa en Éxito (variable 07)
                                                tipoAlerta == TipoAlerta.Error ? mensajeTasaMunicipal : ""),   // {3} - solo se usa en Error (variable 08)
                        PuestoDeTrabajoId = puestoDeTrabajo.Id,
                        NumeroPrograma = s.Programa,
                        NumeroTrama = s.Trama,
                        NumeroVariable = s.Variable,
                        SegundosDeEspera = s.SegundosDeEspera
                    })
                .ToList();

            servicioComandos.Ejecutar(new EnviarMensajesAsincronoCartelLed { Mensajes = mensajes });
        }

        private void AperturaDeBarrera(string codigo)
        {
            try
            {
                servicioOrquestador.Ejecutar(new EjecutarAperturaBarrera { CodigoDispositivo = codigo });
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo levantar la barrera");
            }
        }

        private JsonResult IndexNoGranos(CargaDeCupoDto model, DatosUsuario datosUsuario)
        {
            log.Debug("Asignación de Cupo No Granos {0}, tarjeta {1}, centro {2}", model.Cupo, model.Numero, datosUsuario.CentroId);
            var response = new CargaDeCupoResponseDto();

            model.Fecha = DateTime.Now;
            model.CentroId = datosUsuario.CentroId;
            model.CentroCodigoSap = datosUsuario.CentroCodigoSap;
            var resultado = servicioComandos.Ejecutar(new CrearCargaDeCupoNoGrano { Dto = model }) as ResultadoCrearCargaDeCupo;
            if (resultado.HayErrores)
            {
                var error = resultado.Errores.First();
                response.ValidationErrors.Add(error.Key, error.Value);
            }

            response.Success = resultado.HayErrores && resultado.Errores.ContainsKey("error") ? false : true;

            if (resultado.FastPassValido)
                IniciarWorkflows(resultado, model, resultado.IdPagoMunicipal, resultado.IdExcepcionPagoMunicipal, datosUsuario, resultado.TieneExcepcionTasaMunicipal, resultado.MotivoExcepcionTasaMunicipal);

            ProcesarResultadoTasaMunicipal(resultado.tipoAlerta, resultado.MensajeTasaMunicipal, response);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private void IniciarWorkflows(ResultadoCrearCargaDeCupo resultadoCrearCupoNoGrano, CargaDeCupoDto cargaDeCupo, int? idPagoMunicipal, int idExcepcion, DatosUsuario datosUsuario, bool tieneExcepcionTasaMunicipal, string motivoExcepcionTasaMunicipal)
        {
            var resultadoActividad = new ResultadoCrearWorkflow();
            servicioComandos.Ejecutar(new SetearProgresoCargaDeCupo() { Id = resultadoCrearCupoNoGrano.Id, EnProgresoAutomatico = true });

            switch (cargaDeCupo.TipoOrdenCargaNoGranos)
            {
                case TipoOrdenCargaNoGranos.Insumos:
                    var noProductivosService = factoryNoProductivo.CrearServicio(resultadoCrearCupoNoGrano.FastPassWorkflowDefinicionId);
                    resultadoActividad = noProductivosService.IngresarOrdenCargaInterna(
                        resultadoCrearCupoNoGrano.OrdenCargaInterna,
                        datosUsuario.CentroId,
                        Constantes.WorkFlow.workflowMaterialNoProductivo,
                        resultadoCrearCupoNoGrano.FastPassWorkflowDefinicionId,
                        datosUsuario.NombreUsuario,
                        resultadoCrearCupoNoGrano.ControlRecorrido) as ResultadoCrearWorkflow;
                    break;

                case TipoOrdenCargaNoGranos.Fas:
                    var workflow = Constantes.WorkFlow.workflowVentaFas;
                    var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                    var fasService = factoryFas.CrearServicio(workflowDefinicionId);
                    resultadoActividad = fasService.IngresarOrdenCargaFas(
                        resultadoCrearCupoNoGrano.OrdenCargaFasDto,
                        datosUsuario.CentroId,
                        workflow,
                        workflowDefinicionId,
                        resultadoCrearCupoNoGrano.OrdenCargaFasDto.ValidaCompliance,
                        datosUsuario.NombreUsuario,
                        resultadoCrearCupoNoGrano.ControlRecorrido) as ResultadoCrearWorkflow;
                    break;

                case TipoOrdenCargaNoGranos.FasonConFlete:
                case TipoOrdenCargaNoGranos.FasonSinFlete:
                    var fasonService = factoryFason.CrearServicio(resultadoCrearCupoNoGrano.FastPassWorkflowDefinicionId);
                    resultadoActividad = fasonService.IngresarOrdenCargaInternaFason(
                        resultadoCrearCupoNoGrano.OrdenCargaInternaFason,
                        datosUsuario.CentroId,
                        cargaDeCupo.TipoOrdenCargaNoGranos == TipoOrdenCargaNoGranos.FasonConFlete ?
                            Constantes.WorkFlow.workflowFason :
                            Constantes.WorkFlow.workflowFasonSinFlete,
                        resultadoCrearCupoNoGrano.FastPassWorkflowDefinicionId,
                        datosUsuario.NombreUsuario,
                        resultadoCrearCupoNoGrano.ControlRecorrido) as ResultadoCrearWorkflow;
                    break;

                default:
                    log.Warn($"Se intenta iniciar workflow No Granos desconocido: {cargaDeCupo.TipoOrdenCargaNoGranos}");
                    break;
            }
            
            if (!resultadoActividad.HayErrores)
                EjecutarAccionesDePagoTasaMunicipalPosteriorALaCreacionDeWorkflow(resultadoActividad.InstanciaWorkflowId, idPagoMunicipal, idExcepcion, motivoExcepcionTasaMunicipal, tieneExcepcionTasaMunicipal);

            servicioComandos.Ejecutar(new SetearProgresoCargaDeCupo() { Id = resultadoCrearCupoNoGrano.Id, EnProgresoAutomatico = false });
        }

        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario)
        {
            ViewBag.Items = servicio.ListarDatosDeWorkflowsPendientes(datosUsuario.CentroId, 5);
            return View();
        }

        [DatosUsuario]
        public JsonResult ValidarCupoEnSap(string cupo, string imagen, string nroCartaPorte, DatosUsuario datosUsuario)
        {
            var model = new CargaDeCupoDto();
            var pdfSustentableString = string.Empty;

            if (servicio.CupoConsumido(cupo, datosUsuario.CentroId))
                return Json(new { error = Textos.CupoConsumido }, JsonRequestBehavior.AllowGet);

            try
            {
                var codigosDeCentroSap = servicio.ObtenerCodigoDeCentroPorId(datosUsuario.CentroId);
                log.Debug("ValidarCupoEnSap cupo: {0}, centro: {1}", cupo, string.Join(",", codigosDeCentroSap));
                var esEspecial = false;
                var response = servicioSap.Z_SDMF_RFC_Z2100(new Z_SDMF_RFC_Z2100Request
                {
                    Z_SDMF_RFC_Z2100 = new Z_SDMF_RFC_Z2100()
                    {
                        IM_CENTRO = new ZMPES5210[] { new ZMPES5210 { CENTRO = codigosDeCentroSap[0] } },
                        IM_CODIGO = new ZMPES5200[] { new ZMPES5200 { CODIGO = cupo } }
                    }
                });
                log.Debug(response.ToXml());
                var respuesta = response.Z_SDMF_RFC_Z2100Response.EX_CUPOS.FirstOrDefault();
                if (respuesta != null && respuesta.MENSAJE == Textos.RespuestaSap_NoValido && codigosDeCentroSap.Length > 1)
                {
                    response = servicioSap.Z_SDMF_RFC_Z2100(new Z_SDMF_RFC_Z2100Request
                    {
                        Z_SDMF_RFC_Z2100 = new Z_SDMF_RFC_Z2100()
                        {
                            IM_CENTRO = new ZMPES5210[] { new ZMPES5210 { CENTRO = codigosDeCentroSap[1] } },
                            IM_CODIGO = new ZMPES5200[] { new ZMPES5200 { CODIGO = cupo } }
                        }
                    });
                    respuesta = response.Z_SDMF_RFC_Z2100Response.EX_CUPOS.FirstOrDefault();
                    esEspecial = true;
                }

                if (respuesta != null && respuesta.MENSAJE != Textos.RespuestaSap_NoValido)
                {
                    log.Debug("ValidarCupoEnSap Respuesta {0}: {1}", cupo, respuesta.ToXml());
                    var material = servicio.ObtenerMaterialIdYDescripcionPorCodigoSap(respuesta.MATERIAL.TrimStart(new[] { '0' }));
                    if (material.MaterialId == 0)
                    {
                        ModelState.AddModelError("Cupo", string.Format(Textos.Material_CodigoSAPNoExiste, respuesta.MATERIAL));
                        return Json(new { error = string.Format(Textos.Material_CodigoSAPNoExiste, respuesta.MATERIAL) }, JsonRequestBehavior.AllowGet);
                    }
                    model.MaterialId = material.MaterialId;
                    model.RespuestaSap = respuesta.MENSAJE;
                    model.ProveedorDescripcion = respuesta.DESCPROV;
                    model.ProveedorCuit = respuesta.CUIT;
                    model.MaterialDescripcion = material.Descripcion;
                    model.FechaSap = respuesta.FECHA;
                    model.Especial = esEspecial;
                    model.Camara = respuesta.CALIDAD;

                    if (model.Especial && !String.IsNullOrEmpty(imagen))
                    {
                        imagen = imagen.Replace("data:image/jpg;base64,", string.Empty);
                        var imagenSustentable = Convert.FromBase64String(imagen);
                        imagenSustentable = DibujarSelloSustentable(imagenSustentable);
                        if (imagenSustentable != null)
                        {
                            pdfSustentableString = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(imagenSustentable));
                        }
                    }
                    return Json(new { model, PdfImageSustentableBase64 = pdfSustentableString }, JsonRequestBehavior.AllowGet);
                }

                if (respuesta != null && respuesta.MENSAJE == Textos.RespuestaSap_NoValido && servicio.EsCupoReingresado(cupo, nroCartaPorte, datosUsuario.CentroId))
                {
                    model = servicio.ObtenerCupoReingresado(cupo, nroCartaPorte, datosUsuario.CentroId);
                    model.RespuestaSap = "Cupo a reingresar";
                    var cartaPorte = servicio.ObtenerCartaPortePorCentroYNumero(nroCartaPorte, datosUsuario.CentroId);
                    if (cartaPorte != null)
                    {
                        model.ProveedorCuit = cartaPorte.TitularCartaPorteCuil;
                        model.ProveedorDescripcion = cartaPorte.TitularCartaPorte;
                    }
                    if (model.Especial && !String.IsNullOrEmpty(imagen))
                    {
                        imagen = imagen.Replace("data:image/jpg;base64,", string.Empty);
                        var imagenSustentable = Convert.FromBase64String(imagen);
                        imagenSustentable = DibujarSelloSustentable(imagenSustentable);
                        if (imagenSustentable != null)
                        {
                            pdfSustentableString = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(imagenSustentable));
                        }
                    }
                    return Json(new { model, PdfImageSustentableBase64 = pdfSustentableString }, JsonRequestBehavior.AllowGet);
                }

                log.Debug("ValidarCupoEnSap Respuesta {0} no encontrado", cupo);
                return Json(new { error = respuesta?.MENSAJE }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "Error al validar cupo en SAP: ");
                return Json(new { error = Textos.Error_GenericoSap }, JsonRequestBehavior.AllowGet);
            }
        }

        [DatosUsuario]
        public JsonResult ObtenerCupoCtg(string numeroCartaPorte, string workflow, DatosUsuario datosUsuario)
        {
            try
            {
                log.Debug("Obteniendo CUPO por CP {0} workflow {1}", numeroCartaPorte, workflow);

                var cartaPorteResponse = servicioComandos.Ejecutar(new ConsultarCupoCTG { CentroId = datosUsuario.CentroId, NumeroCartaPorte = numeroCartaPorte, Usuario = datosUsuario.NombreUsuario }) as ResultadoDetalleCTG;
                log.Debug(cartaPorteResponse.HayErrores ? "Error al obtener Cupo CTG{0}: " + cartaPorteResponse.Errores.Values.First() : "Devolviendo Cupo por CP {0}", numeroCartaPorte);

                if (cartaPorteResponse.HayErrores)
                {
                    return Json(new { cartaPorteResponse.CartaPorte, CodigoDeError = cartaPorteResponse.HayErrores ? cartaPorteResponse.Errores.Keys.First() : "0", Error = cartaPorteResponse.Errores.Values.FirstOrDefault() }, JsonRequestBehavior.AllowGet);
                }

                var inhabilitacion = servicio.ListarInhabilitacionCamion(cartaPorteResponse.CartaPorte.Patente, datosUsuario.CentroId);
                if (inhabilitacion.Any())
                {
                    return Json(new { cartaPorteResponse.CartaPorte, CodigoDeError = "3", Error = inhabilitacion }, JsonRequestBehavior.AllowGet);
                }

                var validarCupo = servicio.ValidarCupo(cartaPorteResponse.CartaPorte.Cupo, datosUsuario.CentroId, cartaPorteResponse.CartaPorte.NroCartaPorte);
                if (validarCupo.Valido && validarCupo.YaAsignado)
                {
                    return Json(new { cartaPorteResponse.CartaPorte, CodigoDeError = "2", Error = "El cupo ya esta asignado a otra CP" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { cartaPorteResponse.CartaPorte, CodigoDeError = "0" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener el Cupo CP {0}", numeroCartaPorte);
                throw;
            }
        }

        [DatosUsuario]
        public JsonResult ObtenerFoto(string puestodetrabajoid, string codigoCamara, string directorio, CargaDeCupoDto model, bool fotoPatente = true)
        {
            if (string.IsNullOrEmpty(codigoCamara) || string.IsNullOrEmpty(directorio))
            {
                try
                {
                    var videocamara = ObtenerVideoCamaraPorPuesto(puestodetrabajoid, fotoPatente);
                    codigoCamara = videocamara.Codigo;
                    directorio = videocamara.Directorio;
                }
                catch (Exception e)
                {
                    return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                //Se asume que la primera cámara apunta al frente del camión y la última a la CP
                var resultadoEjecutar = servicioOrquestador.Ejecutar(new EjecutarTomarFoto
                {
                    CodigoDispositivo = codigoCamara
                });

                var resultado = resultadoEjecutar as ResultadoObtenerPatente;
                var resultadoTomarFoto = resultadoEjecutar as ResultadoTomarFoto;
                if (resultado != null)
                {
                    return Json(new
                    {
                        imagen = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(resultado.Imagen)),
                        patente = resultado.Patente,
                        confianza = resultado.Confianza,
                        error = "",
                        directorio = directorio
                    }, JsonRequestBehavior.AllowGet);
                }
                if (resultadoTomarFoto != null)
                {
                    if (!fotoPatente && model != null)
                    {
                        resultadoTomarFoto.Imagen = DibujarEtiqueta(resultadoTomarFoto.Imagen, model);
                    }
                    return Json(new
                    {
                        imagen = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(resultadoTomarFoto.Imagen)),
                        error = "",
                        directorio = directorio
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { error = "No se pudo obtener la imagen" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = "No se pudo obtener la imagen" }, JsonRequestBehavior.AllowGet);
        }

        private VideoCamaraDto ObtenerVideoCamaraPorPuesto(string puestodetrabajoid, bool fotoPatente)
        {
            if (!int.TryParse(puestodetrabajoid, out int puestoId))
                throw new ArgumentException($"PuestoDeTrabajoId inválido: '{puestodetrabajoid}'");

            var puestoDeTrabajo = servicio.ObtenerPuestoDeTrabajo(puestoId);
            if (puestoDeTrabajo?.VideoCamaras == null || !puestoDeTrabajo.VideoCamaras.Any())
                throw new InvalidOperationException($"No hay videocámaras asociadas al puesto {puestoId}");

            return fotoPatente
                ? puestoDeTrabajo.VideoCamaras.OrderBy(x => x.Id).First()
                : puestoDeTrabajo.VideoCamaras.OrderBy(x => x.Id).Last();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarCargaDeCupo { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [DatosUsuario]
        public ActionResult ObtenerMaterial(bool esGrano, DatosUsuario datosUsuario)
        {
            var materiales = servicio.ListarMaterialGranoPorCentro(datosUsuario.CentroId, esGrano).ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);

            return Json(materiales, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerCPEPorPatente(DatosUsuario datosUsuario, string patente, string tarjeta = "", bool esEspecial = false, int? materialId = null)
        {
            try
            {
                log.Debug("Obteniendo CPE por patente {0} en carga de Cupo.", patente);
                servicioComandos.Ejecutar(
                    new RegistrarMarcaDeTiempo 
                    { 
                        Tipo = TipoSensorMarcaTiempo.InicioPorGaritaIngreso, 
                        PuestoDeTrabajoId = datosUsuario .PuestoDeTrabajoId
                    }
                );
                var resultado = servicioComandos.Ejecutar(new ConsultarCPDigital
                { 
                    Patente = patente, 
                    Usuario = datosUsuario.NombreUsuario, 
                    CentroId = datosUsuario.CentroId, 
                    MaterialId = materialId,
                    IncluirImagen = true
                }) as ResultadoCartaPorteElectronica;
                log.Debug(resultado.HayErrores ? "Error al obtener CPE por patente {0}: "+ resultado.Errores.Keys.First() + " - " + resultado.Errores.Values.First() : "Devolviendo CPE por patente {0}", patente);

                // Extraer errores no bloqueantes de TODOS los intervinientes
                var mensajesNoBloqueantes = new List<string>();
                foreach (var clave in ClavesIntervinientesNoBloqueantes.Where(c => resultado.Errores.ContainsKey(c)))
                {
                    mensajesNoBloqueantes.Add(resultado.Errores[clave]);
                    resultado.Errores.Remove(clave);
                }

                if (resultado.HayErrores)
                    return ConstruirJsonResult(new { CodigoDeError = resultado.Errores.Keys.First(), Error = resultado.Errores.Values.First() });

                var codigoErrorNoBloqueante = mensajesNoBloqueantes.Any() ? "3" : null;
                var mensajeErrorNoBloqueante = mensajesNoBloqueantes.Any() ? string.Join(" | ", mensajesNoBloqueantes) : null;

                if (resultado.Duplicados.Any())
                    return ConstruirJsonResult(new { CodigoDeError = nameof(ConsultarCPDigital.MaterialId), Error = "Debe seleccionar Material" });

                if (TryObtenerErrorEstadoCpe(resultado.Cpe, out var codigoEstado, out var mensajeEstado))
                    return ConstruirJsonResult(new { CodigoDeError = codigoEstado, Error = mensajeEstado });

                var etiqueta = new CargaDeCupoDto { Numero = tarjeta, NumeroCartaPorte = resultado.Cpe.CTG };
                var pdfBase64 = string.Empty;
                var pdfSustentableBase64 = string.Empty;

                if (resultado.PdfImage != null)
                {
                    ProcesarPdfConEtiqueta(resultado.PdfImage, etiqueta, esEspecial, out pdfBase64, out pdfSustentableBase64);
                }
                else if (long.TryParse(resultado.Cpe.CTG, out var nroCtg))
                {
                    var imagenCpe = servicioComandos.Ejecutar(new ConsultarImagenCpe { NroCtg = nroCtg }) as ResultadoConsultarImagenCpe;
                    if (!imagenCpe.HayErrores)
                        ProcesarPdfConEtiqueta(imagenCpe.PdfImage, etiqueta, esEspecial, out pdfBase64, out pdfSustentableBase64);
                    else
                        log.Warn("No se pudo obtener la imagen de la CP desde Afip para patente {0}", patente);
                }

                return ConstruirJsonResult(new
                {
                    Cpe = resultado.Cpe,
                    CodigoDeError = codigoErrorNoBloqueante,
                    Error = mensajeErrorNoBloqueante,
                    PdfImageBase64 = pdfBase64,
                    PdfImageSustentableBase64 = pdfSustentableBase64,
                });
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener la carta de porte por patente {0}", patente);
                return ConstruirJsonResult(new { CodigoDeError = string.Empty, Error = Textos.Error_Generico });
            }
        }

        [DatosUsuario]
        public JsonResult ObtenerCPE(DatosUsuario datosUsuario, long numeroCtg, string tarjeta = "", bool esEpecial = false, bool forzarActualizacion = false)
        {
            try
            {
                log.Debug("Obteniendo CTG {0} en carga de Cupo. ForzarActualizacion={1}, Usuario: {2}", numeroCtg, forzarActualizacion, datosUsuario.NombreUsuario);
                var resultado = servicioComandos.Ejecutar(new ConsultarCPDigital 
                { 
                    NroCtg = numeroCtg, 
                    Usuario = datosUsuario.NombreUsuario, 
                    CentroId = datosUsuario.CentroId,
                    IncluirImagen = true,
                    ForzarConsultaAfip = forzarActualizacion
                }) as ResultadoCartaPorteElectronica;

                // Capturar mensajes no bloqueantes de TODOS los intervinientes antes de que sean eliminados
                var mensajesNoBloqueantesCtg = new List<string>();
                foreach (var clave in ClavesIntervinientesNoBloqueantes.Where(c => resultado.Errores.ContainsKey(c)))
                    mensajesNoBloqueantesCtg.Add(resultado.Errores[clave]);

                var codigoError = resultado.HayErrores ? resultado.Errores.Keys.First() : "3";
                var mensajeError = resultado.Errores.Values.FirstOrDefault();
                EliminarDeListaErroresNoBloqueantes(resultado, ref codigoError);

                if (codigoError == "3" && mensajesNoBloqueantesCtg.Any())
                    mensajeError = string.Join(" | ", mensajesNoBloqueantesCtg);

                log.Debug(resultado.HayErrores
                    ? $"Error al obtener CPE {numeroCtg}"
                    : $"CPE {numeroCtg} obtenida correctamente.");

                var pdfBase64 = string.Empty;
                var pdfSustentableBase64 = string.Empty;

                if (codigoError != "2")
                {
                    if (!resultado.HayErrores && TryObtenerErrorEstadoCpe(resultado.Cpe, out var codigoEstado, out var mensajeEstado))
                    {
                        codigoError = codigoEstado;
                        mensajeError = mensajeEstado;
                    }

                    var etiqueta = new CargaDeCupoDto { Numero = tarjeta, NumeroCartaPorte = numeroCtg.ToString() };

                    if (resultado.PdfImage != null)
                    {
                        ProcesarPdfConEtiqueta(resultado.PdfImage, etiqueta, esEpecial, out pdfBase64, out pdfSustentableBase64);
                    }
                    else
                    {
                        var estadoNormalizado = resultado.Cpe?.EstadoCpe?.ToUpper()?.Trim();
                        var esBloqueante = !string.IsNullOrEmpty(estadoNormalizado) && EstadosCPEdeAFIP.Bloqueantes.Any(a => a == estadoNormalizado);

                        if (!esBloqueante)
                        {
                            var imagenCpe = servicioComandos.Ejecutar(new ConsultarImagenCpe { NroCtg = numeroCtg }) as ResultadoConsultarImagenCpe;
                            if (imagenCpe.HayErrores)
                            {
                                mensajeError = "No se pudo obtener la imagen de la CP, por favor tomarlo manualmente.";
                                codigoError = "4";
                            }
                            else
                            {
                                ProcesarPdfConEtiqueta(imagenCpe.PdfImage, etiqueta, esEpecial, out pdfBase64, out pdfSustentableBase64);
                            }
                        }
                    }
                }

                return ConstruirJsonResult(new 
                { 
                    Cpe = resultado.Cpe,
                    CodigoDeError = codigoError,
                    Error = mensajeError,
                    PdfImageBase64 = pdfBase64,
                    PdfImageSustentableBase64 = pdfSustentableBase64,
                });
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener la carta de porte CTG-CPE en carga de Cupo. {0}", numeroCtg);
                return ConstruirJsonResult(new { CodigoDeError = string.Empty, Error = Textos.Error_Generico });
            }
        }

        /// <summary>
        /// Obtiene las órdenes para cualquier worflow posible para completar el campo Materiales cuando es "No Granos".
        /// Pueden venir desde cualquier fuente de datos: MOAOperaciones, SAP
        /// </summary>
        [DatosUsuario]
        public JsonResult ObtenerOrdenesFasonInsumos(string patente, DatosUsuario datosUsuario)
        {
            var response = servicioComandos.Ejecutar(new ConsultarOrdenesNoGranosCargaDeCupo
            {
                Patente = patente,
                CentroId = datosUsuario.CentroId,
            }) as ResultadoConsultarOrdenesNoGranosCargaDeCupo;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private ResultadoEscalables ObtenerTipoVehiculoPorPatente(string patente, string acoplado, string workflow, DatosUsuario datosUsuario, string acoplado2 = "")
        {
            try
            {
                log.Debug("Obteniendo Tipo de vehiculo por patente {0} workflow {1}", patente, workflow);
                return servicioComandos.Ejecutar(new ConsultarEscalables { Patente = patente, Acoplado = acoplado, Acoplado2 = acoplado2, Usuario = datosUsuario.NombreUsuario }) as ResultadoEscalables;
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener el tipo de vehiculo por patente {0}", patente);
                return null;
            }
        }

        private void SetearVista(DatosUsuario datosUsuario)
        {
            var puestoDeTrabajo = servicio.ListarPuestosDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId).Where(x => x.PidePatente).FirstOrDefault();
            if (puestoDeTrabajo != null && puestoDeTrabajo.VideoCamaras != null && puestoDeTrabajo.VideoCamaras.Any())
            {
                var camaraPatente = puestoDeTrabajo.VideoCamaras.OrderBy(x => x.Id).First();
                var camaraCp = puestoDeTrabajo.VideoCamaras.OrderBy(x => x.Id).Last();
                ViewBag.CodigoCamaraPatente = camaraPatente.Codigo;
                ViewBag.CodigoCamaraPatenteDir = camaraPatente.Directorio;
                ViewBag.CodigoCamaraCP = camaraCp.Codigo;
                ViewBag.CodigoCamaraCPDir = camaraCp.Directorio;
            }
            else
            {
                ViewBag.CodigoCamaraPatente = "";
                ViewBag.CodigoCamaraPatenteDir = "";
                ViewBag.CodigoCamaraCP = "";
                ViewBag.CodigoCamaraCPDir = "";
            }
            ViewBag.Materiales = servicio.ListarMaterialesPorWorkflow(225, datosUsuario.CentroId).ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            ViewBag.PuestoDeTrabajo = puestoDeTrabajo != null ? puestoDeTrabajo.ToJson() : null;
            ViewBag.CentroId = datosUsuario.CentroId;
            ViewBag.AvanzaAutomatico = servicio.AvanzaCpe(datosUsuario.CentroId);
        }

        private byte[] DibujarSelloSustentable(byte[] PdfImage)
        {
            if (PdfImage != null)
            {
                var resultado = servicioComandos.Ejecutar(new AgregarMarcaSustentable
                {
                    SoloDibujar = true,
                    PdfImage = PdfImage
                }) as ResultadoCartaPorteElectronica;
                return resultado.PdfImageSustentable;
            }
            return null;
        }

        private byte[] DibujarEtiqueta(byte[] foto, CargaDeCupoDto model, int fontSize = 25)
        {
            var etiqueta = $"Ingreso: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")} Tarjeta: {model.Numero} CP: {model.NumeroCartaPorte}";
            var etiquetad = new Dictionary<string, string>
            {
                {"Ingreso: ", $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}"},
                {"Tarjeta: ", $"{model.Numero}"},
                {"CP: ", $"{model.NumeroCartaPorte}"}
            };


            using (var ms = new MemoryStream(foto))
            using (var imagenCP = new Bitmap(ms))
            {
                PointF posicionEtiqueta = new PointF(0, 0);

                using (Graphics graphics = Graphics.FromImage(imagenCP))
                {
                    using (Font arialFontb = new Font("Arial", fontSize, FontStyle.Bold))
                    {
                        using (Font arialFont = new Font("Arial", fontSize))
                        {
                            var sizeEtiqueta = graphics.MeasureString(etiqueta, arialFont);
                            var rect = new RectangleF(posicionEtiqueta.X, posicionEtiqueta.Y, sizeEtiqueta.Width, sizeEtiqueta.Height);
                            graphics.FillRectangle(Brushes.White, rect);

                            foreach (var k in etiquetad.Keys)
                            {
                                graphics.DrawString(k, arialFont, Brushes.Black, posicionEtiqueta);
                                posicionEtiqueta.X += graphics.MeasureString(k, arialFont).Width;
                                graphics.DrawString(etiquetad[k], arialFontb, Brushes.Black, posicionEtiqueta);
                                posicionEtiqueta.X += graphics.MeasureString(etiquetad[k], arialFontb).Width;
                            }
                        }
                    }
                }
                var resultado = ImageToByte(imagenCP);
                return resultado;
            }
        }

        private static byte[] ImageToByte(Image img)
        {
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(myEncoder, 70L);
            using (var stream = new MemoryStream())
            {
                img.Save(stream, jgpEncoder, myEncoderParameters);
                return stream.ToArray();
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void CargarCartaPorte(int id, DatosUsuario datosUsuario, string imagenCpBase64, ResultadoConsultarPagoTasaMunicipal resultadoTazaMunicipal, CargaDeCupoResponseDto response, string tipoVariedadCodigo)
        {
            log.Info($"CargarCartaPorte: id={id}, datosUsuario={datosUsuario}, imagenCpBase64={imagenCpBase64}, resultadoTazaMunicipal={resultadoTazaMunicipal}, tipoVariedadCodigo={tipoVariedadCodigo}");
            var cargaDeCupo = servicio.ObtenerCupoPorId(id);
            if (cargaDeCupo == null)
            {
                log.Warn("CargarCartaPorte: CargaDeCupo no encontrada para Id={0}", id);
                response.ValidationErrors.Add("avanceCpe", "No hay Carga De Cupo");
                response.Success = false;
                return;
            }

            var puesto = servicio.ObtenerPuestoDeTrabajo(cargaDeCupo.PuestoDeTrabajoId);
            if (!long.TryParse(cargaDeCupo.CTG, out var nroCtg))
            {
                log.Warn("CargarCartaPorte: CTG inválido '{0}' para CargaDeCupo Id={1}", cargaDeCupo.CTG, id);
                response.ValidationErrors.Add("avanceCpe", "El número de CTG tiene un formato inválido");
                response.Success = false;
                return;
            }
            log.Info($"CargarCartaPorte: CTG parseado correctamente: {nroCtg}");
            var orden = servicioComandos.Ejecutar(new ConsultarCPDigital { CentroId = datosUsuario.CentroId, NroCtg = long.Parse(cargaDeCupo.CTG), Usuario = datosUsuario.NombreUsuario }) as ResultadoCartaPorteElectronica;
            if (orden != null && orden.Cpe != null)
            {
                log.Info($"CargarCartaPorte: Carta Porte digital obtenida exitosamente para CTG={nroCtg}");
                var workflow = ObtenerWorkflowSegunTitularCartaPorte(cargaDeCupo, orden);
                var tipoComercialId = ObtenerTipoComercialSegunWorkflow(workflow);
                if (string.IsNullOrEmpty(workflow))
                {
                    log.Warn("CargarCartaPorte: No se encontró workflow para CargaDeCupo Id={0}", id);
                    response.ValidationErrors.Add("avanceCpe", "No hay Carga De Cupo");
                    response.Success = false;
                    return;
                }
                orden.Cpe.TipoVariedadCodigo = tipoVariedadCodigo;
                orden.Cpe.TipoComercialId = tipoComercialId;
                orden.Cpe.Id = 0;
                orden.Cpe.CEE = "99";
                orden.Cpe.CTG = orden.Cpe.NroOrden.ToString().PadLeft(8, '0');
                if (orden.Cpe.Vehiculos != null && orden.Cpe.Vehiculos.Count > 0)
                {
                    orden.Cpe.Vehiculos.FirstOrDefault().Primero = true;
                    var vehiculo = orden.Cpe.Vehiculos.FirstOrDefault();
                    var respuestaCnrt = ObtenerTipoVehiculoPorPatente(vehiculo.Patente, vehiculo.PatenteAcoplado, workflow, datosUsuario, vehiculo.PatenteAcoplado2);
                    // Queda en Pendiente si CNRT no devuelve respuesta o devuelve errores
                    if (respuestaCnrt == null || respuestaCnrt.HayErrores)
                    {
                        log.Warn("CargarCartaPorte: CNRT sin respuesta para patente {0}. CP {1} queda en Pendiente.",
                        vehiculo.Patente, orden.Cpe.NroCartaPorte);
                        response.ValidationErrors.Add("avanceCpe", $"No se obtuvo respuesta del CNRT para la patente {vehiculo.Patente}. CP queda en Pendiente.");
                        response.Success = false;
                        return;
                    }
                    // Queda en Pendiente si supera el Peso Bruto Maximo según el tipo de vehiculo
                    var pesoMaximoPorTipoVehiculo = servicio.ListarPesoMaximoPorTipoVehiculoPorCentro(datosUsuario.CentroId)
                                                            .Where(x => x.TipoVehiculo == (respuestaCnrt.Categoria ?? TipoVehiculo.Camión))
                                                            .Select(x => orden.Cpe.TipoDeWorkflow == TipoDeWorkflow.Ingreso
                                                                        ? x.PesoMaxIngreso
                                                                        : x.PesoMaxEgreso)
                                                            .FirstOrDefault();
                    if (vehiculo.PesoBrutoOrigen > pesoMaximoPorTipoVehiculo)
                    {
                        log.Warn("CargarCartaPorte: CP {0} supera peso bruto máximo ({1} > {2}). Queda en Pendiente.",
                        orden.Cpe.NroCartaPorte, vehiculo.PesoBrutoOrigen, pesoMaximoPorTipoVehiculo);
                        response.ValidationErrors.Add("avanceCpe", $"La CP {orden.Cpe.NroCartaPorte} supera el peso bruto máximo permitido. Queda en Pendiente.");
                        response.Success = false;
                        return;
                    }
                }
                try
                {
                    var path = cargaDeCupo.FotoRutaDestino != null && cargaDeCupo.FotoRutaDestino.Contains("temp")
                             ? Path.GetDirectoryName(cargaDeCupo.FotoRutaDestino)?.Replace("temp", "") ?? string.Empty
                             : string.Empty;
                    CargarAutomaticaCartaPorte(cargaDeCupo, workflow, path, imagenCpBase64, "", orden.Cpe,resultadoTazaMunicipal, datosUsuario, response, orden.Pdf, orden.Errores);
                }
                catch (Exception e)
                {
                    log.Error(e, "CargarAutomaticaCartaPorte falló para CargaDeCupoId={0}, CP={1}",id, orden.Cpe?.NroCartaPorte);
                    response.ValidationErrors.Add("avanceCpe", "Error interno al procesar la Carta de Porte. CP queda en Pendiente.");
                    response.Success = false;
                }
            }
            else
            {
                log.Warn("CargarCartaPorte: Sin respuesta CPE para CargaDeCupo Id={0}, CTG={1}", id, cargaDeCupo.CTG);
                response.ValidationErrors.Add("avanceCpe", "No se pudo obtener la Carta de Porte electrónica.");
                response.Success = false;
                return;
            }
        }

        private void CargarAutomaticaCartaPorte(CargaDeCupoDto cargaDeCupo, string workflow, string path, string imagenCpBase64, string fotoMesaDigitalizacion2, CartaPorteDto orden, ResultadoConsultarPagoTasaMunicipal resultadoTazaMunicipal, DatosUsuario datosUsuario, CargaDeCupoResponseDto responseCargaDeCupo, byte[] pdf, IDictionary<string, string> erroresIntervinientes = null)
        {
            log.Debug("Iniciando Carga de Carta de Porte número {0}", orden.NroCartaPorte);
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            if (workflowObj == null)
            {
                log.Error("No se encontró workflow con código {0}", workflow);
                responseCargaDeCupo.ValidationErrors.Add("avanceCpe", $"Workflow '{workflow}' no encontrado.");
                responseCargaDeCupo.Success = false;
                return;
            }

            var vehiculos = orden.Vehiculos?.ToList();

            if (orden.Cpe && workflowObj.TipoDeWorkflow == TipoDeWorkflow.Egreso)
            {
                if (string.IsNullOrEmpty(orden.NroCartaPorte))
                {
                    var sequenciaNroCartaPorteCPE = servicio.ObtenerSequenciaNumeroCTGCartaPorteElectronica();
                    orden.NroCartaPorte = $"{DateTime.Now.ToString("yyyyMMdd")}{sequenciaNroCartaPorteCPE.ToString("D4")}";
                }
            }
            if (datosUsuario.CentroId == 0)
            {
                log.Debug("El usuario {0} no tiene seleccionado un centro", datosUsuario.NombreUsuario);
                responseCargaDeCupo.ValidationErrors.Add("avanceCpe", $"El usuario {datosUsuario.NombreUsuario} no tiene seleccionado un centro");
                responseCargaDeCupo.Success = false;
                return;
            }
            if (!EsAptoParaAvanceAutomatico(orden, datosUsuario, erroresIntervinientes))
            {
                log.Debug("No apto para avance automático");
                responseCargaDeCupo.Success = false;
                return;
            }
            if (responseCargaDeCupo.Success && vehiculos?.Count > 0)
            {
                var response = servicio.NumeroCartaPorteValido(orden.NroCartaPorte, datosUsuario.CentroId, workflowObj.Descripcion, orden.Cpe);
                if (!response.Valida)
                {
                    log.Debug("No se puede crear la CP {0}. Detalle: {1}", orden.NroCartaPorte, response.Error);
                    responseCargaDeCupo.ValidationErrors.Add("avanceCpe", $"No se puede crear la CP {orden.NroCartaPorte}. Detalle: {response.Error}");
                    responseCargaDeCupo.Success = false;
                    return;
                }

                if (vehiculos.Count() != vehiculos.GroupBy(x => x.Patente).Count())
                {
                    log.Debug("No se puede crear la CP {0}. Alguna de las patentes está duplicada");
                    responseCargaDeCupo.ValidationErrors.Add("avanceCpe", $"No se puede crear la CP. Alguna de las patentes está duplicada");
                    responseCargaDeCupo.Success = false;
                    return;
                }

                if (vehiculos.Any(vehiculo => workflows.ObtenerWorkflowPorPatente(vehiculo.Patente) != null))
                {
                    log.Debug("No se puede crear la CP. Alguna de las patentes esta ingresada en un workflow en ejecución");
                    responseCargaDeCupo.ValidationErrors.Add("avanceCpe", "No se puede crear la CP. Alguna de las patentes esta ingresada en un workflow en ejecución");
                    responseCargaDeCupo.Success = false;
                    return;
                }

                var tipoComercial = servicio.ObtenerTipoComercial(orden.TipoComercialId);
                if (tipoComercial?.PesoMaximoDocumentoIngreso != null && tipoComercial?.PesoMaximoDocumentoIngreso != 0)
                {
                    if (vehiculos.Any(vehiculo => vehiculo.PesoBrutoOrigen > tipoComercial.PesoMaximoDocumentoIngreso))
                    {
                        log.Debug("El Tipo comercial tiene configurado un peso maximo en ingreso y fue excedido");
                        responseCargaDeCupo.ValidationErrors.Add("avanceCpe", "El Tipo comercial tiene configurado un peso maximo en ingreso y fue excedido");
                        responseCargaDeCupo.Success = false;
                        return;
                    }
                }
                var primerVehiculo = vehiculos[0];  // seguro post-ToList() con guard Count > 0
                var tipoVehiculo = ObtenerTipoVehiculoPorPatente(
                    primerVehiculo.Patente, primerVehiculo.PatenteAcoplado,
                    workflow, datosUsuario, primerVehiculo.PatenteAcoplado2);

                if (tipoVehiculo == null)
                {
                    log.Warn("CargarAutomaticaCartaPorte: CNRT no devolvió tipo de vehículo para patente {0}", primerVehiculo.Patente);
                    responseCargaDeCupo.ValidationErrors.Add("avanceCpe", "No se pudo determinar el tipo de vehículo");
                    responseCargaDeCupo.Success = false;
                    return;
                }
                if (tipoVehiculo.HayErrores)
                {
                    log.Warn("CargarAutomaticaCartaPorte: Error de CNRT para patente {0}", primerVehiculo.Patente);
                    responseCargaDeCupo.ValidationErrors.Add("avanceCpe", "Fallo validación tipo vehículo");
                    responseCargaDeCupo.Success = false;
                    return;
                }
                orden.TipoVehiculo = tipoVehiculo.Categoria.HasValue ? tipoVehiculo.Categoria.Value : TipoVehiculo.Camión;

                var resultadoChofer = SetearChofer(orden.Chofer);
                if (resultadoChofer == false)
                {
                    log.Debug("No se pudo dar de alta o asociar el chofer a la CP");
                    responseCargaDeCupo.ValidationErrors.Add("avanceCpe", "No se pudo dar de alta o asociar el chofer a la CP");
                    responseCargaDeCupo.Success = false;
                    return;
                }

                var transportistaId = orden.TransportistaId ?? 0;
                var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
                orden.TransportistaId = transportistaId;
                if (!resultadoTransportista)
                {
                    log.Debug("No se pudo dar de alta o asociar el transportista a la CP");
                    responseCargaDeCupo.ValidationErrors.Add("avanceCpe", "No se pudo dar de alta o asociar el transportista a la CP");
                    responseCargaDeCupo.Success = false;
                    return;
                }

                if (!ValidarCupo(orden, datosUsuario, workflowObj.TipoDeWorkflow == TipoDeWorkflow.Ingreso))
                {
                    log.Debug($"La {orden.NroCartaPorte} se queda en pendiente por tener establecimiento asociado al titular de Carta de Porte");
                    responseCargaDeCupo.ValidationErrors.Add("avanceCpe", "Cupo ya asignado");
                    responseCargaDeCupo.Success = false;
                    return;
                }

                orden.Vehiculos = vehiculos;
                orden.FechaEmision = DateTime.Now;
                orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;
                orden.EsClienteDestinatario = false;
                orden.CodEstab = string.IsNullOrEmpty(orden.CodEstab) ? "999999" : orden.CodEstab;
                var i = 1;
                foreach (var vehiculo in vehiculos)
                {
                    vehiculo.TipoVehiculo = orden.TipoVehiculo;
                    vehiculo.Patente = vehiculo.Patente != null ? vehiculo.Patente.ToUpper() : "";
                    vehiculo.PatenteAcoplado = vehiculo.PatenteAcoplado != null ? vehiculo.PatenteAcoplado.ToUpper() : "";
                    vehiculo.PatenteAcoplado2 = vehiculo.PatenteAcoplado2 != null ? vehiculo.PatenteAcoplado2.ToUpper() : "";
                    vehiculo.NumeroVehiculo = i++;
                }

                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(imagenCpBase64))
                {
                    orden.FotoRutaDestino = GuardarfotoMesaDigitalizacion(imagenCpBase64, orden, path, datosUsuario, DateTime.Now);
                    if (cargaDeCupo.Especial)
                    {
                        orden.FotoRutaSustentable = GuardarfotoMesaDigitalizacionSelloSustentable(orden, datosUsuario, DateTime.Now);
                    }
                }
                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);

                log.Info("CargarCartaPorte: Iniciando carga de workflow/s para los/el vehiculo/s: " + orden.VehiculoJson);
                foreach (var vehiculo in vehiculos)
                {
                    var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActCargarCartaPorte,
                        ActividadXaml = "CargarCartaPorte",
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };
                    var resultadoActividad = servicioWf.CargarCartaPorte(orden, vehiculo, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
                    if (resultadoActividad.HayErrores)
                    {
                        log.Debug(resultadoActividad.Errores.FirstOrDefault().Value);
                        responseCargaDeCupo.ValidationErrors.Add("avanceCpe", resultadoActividad.Errores.FirstOrDefault().Value);
                        responseCargaDeCupo.Success = false;
                        return;
                    }
                    orden.Id = resultadoActividad.Id;

                    GuardarDocumentoPorRecorrido(pdf, resultadoActividad.InstanciaWorkflowId);
                    EjecutarAccionesDePagoTasaMunicipalPosteriorALaCreacionDeWorkflow(resultadoActividad.InstanciaWorkflowId, resultadoTazaMunicipal?.IdPago, resultadoTazaMunicipal.IdExcepcion, resultadoTazaMunicipal.MotivoExcepcion, resultadoTazaMunicipal.TieneExcepcion);
                }

                if (responseCargaDeCupo.Success)
                {
                    cargaDeCupo.IngresoAvanceCPEAutomatico = true;
                    servicioComandos.Ejecutar(new ModificarCargaDeCupo { Dto = cargaDeCupo });
                }
            }
        }

        protected virtual bool EsAptoParaAvanceAutomatico(CartaPorteDto orden, DatosUsuario usuario, IDictionary<string, string> erroresIntervinientes)
        {
            // Errores de intervinientes (ej: proveedor no encontrado en SAP) no bloquean la consulta,
            // pero sí deben bloquear el avance automático: la CP queda en Pendiente.
            if (erroresIntervinientes != null)
            {
                var erroresDeIntervinientesNoBloqueantes = erroresIntervinientes
                    .Where(e => ClavesIntervinientesNoBloqueantes.Contains(e.Key))
                    .ToList();
                if (erroresDeIntervinientesNoBloqueantes.Any())
                {
                    string mensajeError = string.Join("; ", erroresDeIntervinientesNoBloqueantes.Select(e => e.Value)) + ". CP queda en Pendiente.";
                    log.Warn("EsAptoParaAvanceAutomatico: errores de intervinientes: {0}", mensajeError);
                    return false;
                }
            }

            var codigoSapMolinosAgro = firma.ObtenerFirmaSinLogo().CodigoSAP;
            var codigoSapMRP = ConfigurationManager.AppSettings["CodigoSapMRP"];
            var codigoSapTitular = servicio.ObtenerProveedor(orden.TitularCartaPorteId)?.CodigoSap;
            var codigoSapRemitente = servicio.ObtenerProveedor(orden.RtteComercialId)?.CodigoSap;
            var otroRecorridoDelChofer = orden.Chofer != null ? servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id) : null;
            var escenario = WorkflowCartaPorteHelper.ObtenerEscenario(
                codigoSapTitular,
                codigoSapRemitente,
                orden.DestinoCodigoSap,
                orden.DestinatarioCodigoSap,
                codigoSapMolinosAgro,
                codigoSapMRP,
                orden.CodEstab,
                orden.RtteComercialVentaSecundarioCuil);

            if (escenario == EscenarioWorkflowCartaPorte.Compra)
                return true;

            if (escenario == EscenarioWorkflowCartaPorte.Redespacho)
                return false;

            if (otroRecorridoDelChofer != null && !(orden.TipoVehiculo == TipoVehiculo.Tren))
                return false;

            if (orden.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EPA)
            {
                if (usuario.CentroId == Constantes.Centro.IdSanLorenzo && orden.MaterialCodigoSap == Constantes.MaterialPagoRealizado.SojaSAP)
                    return false;

                if (usuario.CentroId == Constantes.Centro.IdSanLorenzo && orden.DestinatarioCuil == Constantes.Proveedores.CuitMolinos)
                    return false;

                if (!servicio.EsProveedorSustentable(orden.TitularCartaPorteId))
                    return false;
            }

            return true;
        }

        private bool ValidarCupo(CartaPorteDto orden, DatosUsuario usuario, bool esIngreso)
        {
            //No validamos Si no requiere cupo o si el destino no es un centro de MOA
            if (!orden.RequiereCupo || !orden.ValidarCupo || (!esIngreso && orden.EsClienteDestinatario) || (!esIngreso && !orden.Cupo.StartsWith("MOL")))
            {
                return true;
            }

            var resultado = servicio.ValidarCupo(orden.Cupo, usuario.CentroId, orden.NroCartaPorte);

            if (resultado.YaAsignado)
            {
                log.Debug(resultado.MensajeError);
                return false;
            }

            return true;
           
        }

        private string GuardarfotoMesaDigitalizacion(string fotoMesaDigitalizacion, CartaPorteDto orden, string directorio, DatosUsuario datosUsuario, DateTime fecha, string numCtg = null)
        {
            if (!string.IsNullOrEmpty(fotoMesaDigitalizacion))
            {
                log.Debug($"GuardarfotoMesaDigitalizacion  {fotoMesaDigitalizacion.Length} {directorio} {fecha}");
                var path = servicioComandos.Ejecutar(
                    new GuardarfotoMesaDigitalizacion
                    {
                        Fecha = fecha,
                        FotoMesaDigitalizacion = fotoMesaDigitalizacion,
                        Directorio = directorio,
                        CentroId = datosUsuario.CentroId,
                        NumeroDocumentoIngreso = !string.IsNullOrEmpty(numCtg) ? numCtg : orden.NroCartaPorte,
                        TipoVehiculo = orden.Vehiculos.First().TipoVehiculo,
                        Usuario = datosUsuario.NombreUsuario,
                        Patente = orden.Vehiculos.First().Patente
                    }) as ResultadoGuardarFoto;

                return path != null ? path.Path : null;
            }
            return null;
        }

        protected bool SetearChofer(ChoferDto choferDto)
        {
            if (!ModelState.IsValid || choferDto == null)
            {
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
                    return false;
                }
                choferDto.Id = (resultadoChofer as ResultadoCrear).Id;
            }
            return true;
        }

        protected bool SetearTransportista(ref int transportistaId, int tipoComercialId, bool esTransportista)
        {
            var tipoComercial = servicio.ObtenerTipoComercial(tipoComercialId);
            if (tipoComercial.TransportistaEsProveedor && (transportistaId == 0))
            {
                return false;
            }

            if (!esTransportista)
            {
                var proveedor = servicio.ObtenerProveedor(transportistaId);
                if (proveedor == null)
                {
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
                            log.Error("SetearTransportista: resultado inesperado de tipo {0}",
                                resultadoTransportista?.GetType()?.Name ?? "null");
                            return false;
                        }

                        transportistaId = (resultadoTransportista as ResultadoCrear).Id;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "SetearTransportista: error al crear transportista");
                        return false;
                    }
                }
            }
            return true;
        }

        private string GuardarfotoMesaDigitalizacionSelloSustentable(CartaPorteDto orden, DatosUsuario datosUsuario, DateTime fecha, string numCtg = null)
        {
            if (!string.IsNullOrEmpty(orden.FotoRutaDestino))
            {
                log.Debug($"GuardarfotoMesaDigitalizacionSustentable {orden.FotoRutaDestino} {fecha}");
                var resultadoSustentable = servicioComandos.Ejecutar(new AgregarMarcaSustentable
                {
                    RutaFotoCP = orden.FotoRutaDestino,
                    CodigoCentroSap = datosUsuario.CentroCodigoSap,
                    NroDocumento = orden.NroCartaPorte,
                    Patente = orden.Patente,
                    SoloDibujar = false
                }) as ResultadoGuardarFoto;
                return resultadoSustentable != null ? resultadoSustentable.Path : null;
            }
            return null;
        }

        private bool EsCupoValidoParaAvanceAutomatico(CargaDeCupoDto model, string tipoVariedadCodigo)
        {
            bool esValido = true;

            if (model.Especial && model.MaterialId == 4 && tipoVariedadCodigo != Constantes.TipoVariedadMaterial.EUDR) // CUPO SUSTENTABLE, EPA o EPA/EUDR
                esValido = false;
            else if (model.SinCupo && model.Cupo == Constantes.ValoresPorDefecto.CupoGenerico) // CUPO GENERICO
                esValido = false;

            log.Info($"EsCupoValidoParaAvanceAutomatico: Especial={model.Especial}, MaterialId={model.MaterialId}, TipoVariedadCodigo={tipoVariedadCodigo}, SinCupo={model.SinCupo}, Cupo={model.Cupo}, EsValido={esValido}");
            return esValido;
        }

        private bool PermitirAsignarCalleGrano(string codigoSapTitularCartaPorte, string codigoEstablecimiento, string remitenteComercialCodigoSap = null, string remitenteComercialVentaSecundariaCuit = null)
        {
            var codigoSapPuertoRosario = ConfigurationManager.AppSettings["CodigoSapPuertoRosario"];
            var codigoSapMolinosAgro = firma.ObtenerFirmaSinLogo().CodigoSAP;
            return codigoSapTitularCartaPorte != codigoSapPuertoRosario
                && !(codigoSapTitularCartaPorte == Constantes.ValoresPorDefecto.CodigoSapACA
                    && codigoEstablecimiento == Constantes.ValoresPorDefecto.EstablecimientoACA
                    && ((!string.IsNullOrEmpty(remitenteComercialCodigoSap) && remitenteComercialCodigoSap == codigoSapMolinosAgro)
                        || (!string.IsNullOrEmpty(remitenteComercialVentaSecundariaCuit) && remitenteComercialVentaSecundariaCuit == Constantes.Proveedores.CuitMolinos)));
        }

        private string ObtenerWorkflowSegunTitularCartaPorte(CargaDeCupoDto cargaDeCupo, ResultadoCartaPorteElectronica orden)
        {
            var codigoSapMrp = ConfigurationManager.AppSettings["CodigoSapMRP"];
            var codigoSapMolinosAgro = firma.ObtenerFirmaSinLogo().CodigoSAP;
            var workflowCompraGranos = ConfigurationManager.AppSettings["WorkflowIngresoPorCompra"];
            var workflowRedespachoGranos = ConfigurationManager.AppSettings["workflowRedespacho"];
            var workflowImportacionGranos = ConfigurationManager.AppSettings["workflowIngresoPorImpoGranos"];

            var escenario = WorkflowCartaPorteHelper.ObtenerEscenario(
                cargaDeCupo.TitularCartaPorteCodigoSap,
                cargaDeCupo.RtteComercialCodigoSap,
                orden?.Cpe?.DestinoCodigoSap,
                orden?.Cpe?.DestinatarioCodigoSap,
                codigoSapMolinosAgro,
                codigoSapMrp,
                cargaDeCupo.CodEstab,
                cargaDeCupo.RtteComercialVentaSecundariaCuit);

            if (escenario == EscenarioWorkflowCartaPorte.Compra)
            {
                log.Info($"ObtenerWorkflowSegunTitularCartaPorte: workflowCompraGranos={workflowCompraGranos}");
                return workflowCompraGranos;
            }

            if (escenario == EscenarioWorkflowCartaPorte.Redespacho)
            {
                log.Info($"ObtenerWorkflowSegunTitularCartaPorte: workflowRedespachoGranos={workflowRedespachoGranos}");
                return workflowRedespachoGranos;
            }

            if (escenario == EscenarioWorkflowCartaPorte.Importacion)
            {
                log.Info($"ObtenerWorkflowSegunTitularCartaPorte: workflowImportacionGranos={workflowImportacionGranos}");
                return workflowImportacionGranos;
            }

            return string.Empty;
        }

        private int ObtenerTipoComercialSegunWorkflow(string workflow)
        {
            var tipoComercialId = 0;
            const int TIPO_COMERCIAL_REDESPACHO = 8;
            const int TIPO_COMERCIAL_COMPRA_GRANOS = 4;

            var workflowRedespacho = ConfigurationManager.AppSettings["workflowRedespacho"];
            var workflowImpoGranos = ConfigurationManager.AppSettings["workflowIngresoPorImpoGranos"];
            var workflowCompraGranos = ConfigurationManager.AppSettings["WorkflowIngresoPorCompra"];

            if (workflow == workflowRedespacho || workflow == workflowImpoGranos)
            {
                tipoComercialId = TIPO_COMERCIAL_REDESPACHO;
            }
            else if (workflow == workflowCompraGranos)
            {
                tipoComercialId = TIPO_COMERCIAL_COMPRA_GRANOS;
            }

            return tipoComercialId;
        }

        private void ProcesarResultadoTasaMunicipal(TipoAlerta tipoAlerta, string mensajeAlerta, CargaDeCupoResponseDto response)
        {
            response.Data.MensajeTasaMunicipal = mensajeAlerta;
            response.Data.TipoAlertaTasaMunicipal = tipoAlerta;
        }

        private void ActualizarTasaMunicipal(Guid InstanciaWorkflowId, int? IdPago)
        {
            try
            {
                var respuestaTasa = servicioComandos.Ejecutar(new ModificarComoUsadoPagosTasaMunicipal
                {
                    InstanceId = InstanciaWorkflowId,
                    PagoId = IdPago.Value
                });
            }
            catch (Exception e)
            {
                log.Error(e, "Error al modificar Tasa Municipal");
            }
        }

        private Resultado ValidarVisec(string cosecha, int materialId, int? pesoNeto, string codEstab, string codigoRENSPA)
        {
            var resultadoStockVisec = new Resultado();
            if (!pesoNeto.HasValue)
            {
                resultadoStockVisec.Error(string.Empty, "El peso neto es obligatorio para validar el stock en VISec");
                return resultadoStockVisec;
            }

            var plantaOrigen = int.Parse(codEstab);
            var tipoOrigenCPE = VisecHelper.ObtenerTipoOrigenCPE(plantaOrigen);
            var campania = cosecha.Replace("-", "/");
            if (campania.Length == 4)
                campania = campania.Insert(2, "/");

            var material = servicio.ObtenerMaterial(materialId);
            if (tipoOrigenCPE == TipoOrigenCPE.RUCA)
            {
                resultadoStockVisec = servicioComandos.Ejecutar(new ConsultarStockRUCA
                {
                    Campania = campania,
                    CodigoProductoAFIP = int.Parse(material.CodigoONCCA ?? "0"),
                    CUITEmpresaResponsable = Constantes.ValoresPorDefecto.CuitMOA.ToString(),
                    Volumen = pesoNeto.Value,
                    NumeroRUCATitular = plantaOrigen,
                });

                if (resultadoStockVisec.HayErrores)
                    return resultadoStockVisec;
            }
            else
            {
                if (string.IsNullOrEmpty(codigoRENSPA))
                {
                    resultadoStockVisec.Error(string.Empty, "No se pudo obtener el código RENSPA");
                    return resultadoStockVisec;
                }

                resultadoStockVisec = servicioComandos.Ejecutar(new ConsultarStockUP
                {
                    Campania = campania,
                    CodigoProductoAFIP = int.Parse(material.CodigoONCCA ?? "0"),
                    CUITEmpresaResponsable = Constantes.ValoresPorDefecto.CuitMOA.ToString(),
                    Volumen = pesoNeto.Value,
                    NumeroRENSPA = codigoRENSPA,
                });

                if (resultadoStockVisec.HayErrores)
                    return resultadoStockVisec;
            }

            return resultadoStockVisec;
        }

        private void ActualizarExcepcionPorPatente(Guid workflowInstanceId, int idExcepcion)
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
                log.Error(e, "Error al modificar Tasa Municipal");
            }
        }

        private void MarcarRecorridoComoContingencia(Guid InstanciaWorkflowId)
        {
            try
            {
                if (InstanciaWorkflowId != Guid.Empty)
                {
                    servicioComandos.Ejecutar(new ModificarRecorridoPorContingenciaPay
                    {
                        InstanceId = InstanciaWorkflowId
                    });
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Error al modificar Tasa Municipal");
            }
        }

        private void InformarPagoTasaMunicipal(Guid InstanciaWorkflowId)
        {
            try
            {
                servicioComandos.Ejecutar(new ModificarInformadoPagosTasaMunicipal
                {
                    InstanceId = InstanciaWorkflowId
                });
            }
            catch (Exception e)
            {
                log.Error(e, "Error al modificar Tasa Municipal");
            }
        }

        private void CrearRecorridoTasaMunicipal(Guid instanceId, string motivoExcepcion, bool tieneExcepcion)
        {
            try
            {
                var idRecorrido = servicio.ObtenerRecorridoIdPorGuid(instanceId);
                var respuestaCrearRecorridoTasaMunicipal = servicioComandos.Ejecutar(new CrearModificarRecorridoTasaMunicipal
                {
                    Id = idRecorrido,
                    MotivoExcepcion = motivoExcepcion,
                    TieneExcepcion = tieneExcepcion
                });
            }
            catch (Exception e)
            {
                log.Error(e, "Error al crear recorrido Tasa Municipal");
            }
        }

        private void EjecutarAccionesDePagoTasaMunicipalPosteriorALaCreacionDeWorkflow(Guid workflowInstanceId, int? idPagoMunicipal, int idExcepcion, string motivoExcepcionTasaMunicipal, bool tieneExcepcionTasaMunicipal)
        {
            if (idPagoMunicipal.HasValue && idPagoMunicipal.Value > 0)
                ActualizarTasaMunicipal(workflowInstanceId, idPagoMunicipal);

            if (idExcepcion > 0)
            {
                InformarPagoTasaMunicipal(workflowInstanceId);
                ActualizarExcepcionPorPatente(workflowInstanceId, idExcepcion);
            }

            CrearRecorridoTasaMunicipal(workflowInstanceId, motivoExcepcionTasaMunicipal, tieneExcepcionTasaMunicipal);

            if (servicio.TieneContingenciaPorTipo(Constantes.Contingencia.PayCaido))
                MarcarRecorridoComoContingencia(workflowInstanceId);
        }

        private void GuardarDocumentoPorRecorrido(byte[] pdf, Guid instanceId)
        {
            try
            {
                var documentoPorRecorrido = new CrearDocumentoPorRecorrido
                {
                    Archivo = pdf,
                    ArchivoExtension = "pdf",
                    WorkflowIntanceId = instanceId,
                    TipoDocumentoIngreso = TipoImpresion.CartaDePorteElectronica,
                    Fecha = DateTime.Now
                };

                var respuestaCrearDocumento = servicioComandos.Ejecutar(documentoPorRecorrido);
                if (respuestaCrearDocumento.HayErrores)
                {
                    log.Error("Error al crear documento por recorrido - {0}", respuestaCrearDocumento.Errores.FirstOrDefault().Value);
                }
            }
            catch (Exception e)
            {
                log.Error("Error al guardar documento pdf - {0}", e.Message);
            }
        }

        private JsonResult ConstruirJsonResult(object data) => new JsonResult
        {
            Data = data,
            ContentType = "application/json",
            ContentEncoding = System.Text.Encoding.UTF8,
            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            MaxJsonLength = int.MaxValue
        };

        // Returns true when the CPE state is invalid, populating codigoError and mensajeError.
        private bool TryObtenerErrorEstadoCpe(CartaPorteDto cpe, out string codigoError, out string mensajeError)
        {
            codigoError = null;
            mensajeError = null;

            if (cpe == null || EstadosCPEdeAFIP.Validos.Contains(cpe.EstadoCpe))
                return false;

            var estadoCPE = cpe.EstadoCpe?.ToUpper()?.Trim();
            if (!string.IsNullOrEmpty(estadoCPE) && EstadosCPEdeAFIP.Bloqueantes.Any(a => a == estadoCPE))
            {
                codigoError = "5";
                mensajeError = $"El CTG {cpe.CTG} se encuentra en estado {(EstadosCPEdeAFIP.Descripciones.ContainsKey(estadoCPE) ? EstadosCPEdeAFIP.Descripciones[estadoCPE] : estadoCPE)}";
                return true;
            }

            codigoError = "4";
            mensajeError = $"El CTG {cpe.CTG} no se encuentra en estado ACTIVO";
            return true;
        }

        // Draws the label and optionally the sello sustentable on pdfImage, returning base64 strings.
        private void ProcesarPdfConEtiqueta(byte[] pdfImage, CargaDeCupoDto etiqueta, bool esEspecial, out string pdfBase64, out string pdfSustentableBase64)
        {
            pdfBase64 = string.Empty;
            pdfSustentableBase64 = string.Empty;

            if (pdfImage == null) return;

            var pdfConEtiqueta = DibujarEtiqueta(pdfImage, etiqueta, 18);
            pdfBase64 = $"data:image/jpg;base64,{Convert.ToBase64String(pdfConEtiqueta)}";

            if (esEspecial)
            {
                var pdfSustentable = DibujarSelloSustentable(pdfConEtiqueta);
                if (pdfSustentable != null)
                    pdfSustentableBase64 = $"data:image/jpg;base64,{Convert.ToBase64String(pdfSustentable)}";
            }
        }

        private void EliminarDeListaErroresNoBloqueantes(Resultado resultado, ref string codigoError)
        {
            foreach (var clave in ClavesIntervinientesNoBloqueantes)
            {
                if (resultado.Errores.ContainsKey(clave))
                {
                    resultado.Errores.Remove(clave);
                    codigoError = "3";
                }
            }
        }

        private string GenerarImagenCartaPorte(CargaDeCupoDto model)
        {
            if (!long.TryParse(model.CTG, out var nroCtg))
            {
                log.Warn("CTG no numérico en GenerarImagenCartaPorte: '{0}'. Usando imagen del formulario.", model.CTG);
                return model.Especial
                    ? (model.ImagenCartaPorteSustentable ?? string.Empty).Replace("data:image/jpg;base64,", "")
                    : (model.ImagenCartaPorte ?? string.Empty).Replace("data:image/jpg;base64,", "");
            }
            var cartaPorteImagen = servicioComandos.Ejecutar(new ConsultarImagenCpe { NroCtg = nroCtg }) as ResultadoConsultarImagenCpe;
            if (cartaPorteImagen.HayErrores)
            {
                log.Debug("No se pudo obtener la imagen de la CP desde cache");
                return model.Especial
                    ? (model.ImagenCartaPorteSustentable ?? string.Empty).Replace("data:image/jpg;base64,", "")
                    : (model.ImagenCartaPorte ?? string.Empty).Replace("data:image/jpg;base64,", "");
            }
            else
            {
                if (model.Especial)
                {
                    log.Debug("Generando imagen de carta porte sustentable");
                    var cartaPdfSustentable = DibujarSelloSustentable(cartaPorteImagen.PdfImage);
                    if (cartaPdfSustentable == null)
                    {
                        log.Warn("No se pudo generar sello sustentable para CTG {0}", model.CTG);
                        return (model.ImagenCartaPorteSustentable ?? string.Empty).Replace("data:image/jpg;base64,", "");
                    }
                    return Convert.ToBase64String(cartaPdfSustentable);
                }
                
                log.Debug("Generando imagen de carta porte");
                var cartaPdf = DibujarEtiqueta(cartaPorteImagen.PdfImage, new CargaDeCupoDto()
                {
                    Numero = model.Numero,
                    NumeroCartaPorte = model.NumeroCartaPorte,
                }, 18);
                var pdfString = Convert.ToBase64String(cartaPdf);
                
                return pdfString;
            }
        }
    }
}