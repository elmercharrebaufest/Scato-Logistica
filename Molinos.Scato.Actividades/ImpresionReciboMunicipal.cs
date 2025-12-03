using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Impl;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Actividades
{
    public class ImpresionReciboMunicipal : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        public InArgument<int?> CantCopias { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var log = context.GetExtension<ILogger>();
            var resultado = new Resultado();
            var centroId = CentroId.Get<int>(context);
            var workflowId = WorkflowId.Get<Guid>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            log.Info($"Iniciando actividad de impresión de recibo municipal para el workflow {workflowId} en el centro {centroId} y puesto de trabajo {puestoDeTrabajoId}.");

            try
            {
                var parametros = ObtenerParametros(context);
                log.Info($"Iniciando actividad de impresión de recibo municipal para el workflow {parametros.WorkflowId} en el centro {parametros.CentroId} y puesto de trabajo {parametros.PuestoDeTrabajoId}.");

                CrearLogActividad(servicio, parametros.WorkflowId, log, "Validar impresion Recibo Municipal");
                var documento = ObtenerDocumentoDeImpresion(repositorio, parametros, log);
                var recorrido = ObtenerYValidarRecorrido(repositorio, parametros.WorkflowId, log);
                var aplicaPago = DeterminarSiAplicaPago(repositorio, recorrido, parametros);
                log.Info($"Recorrido obtenido: {recorrido.Id}, Patente: {recorrido.Patente}, Aplica pago: {aplicaPago}.");

                var datosRecorrido = repositorio.ObtenerRecorridoImpresionReciboMunicipal(parametros.WorkflowId);
                if (datosRecorrido == null)
                {
                    throw new Exception($"No se encontraron datos de recorrido para el workflow {parametros.WorkflowId}.");
                }

                var pagos = ObtenerPagosDigitales(repositorio, parametros.WorkflowId);
                if (pagos != null && pagos.Count > 0)
                {
                    CrearLogActividad(servicio, parametros.WorkflowId, log, "Pago Tasa Municipal Digital");
                    log.Info($"Se encontraron {pagos.Count} pagos digitales asociados al recorrido.");
                    foreach (var pago in pagos)
                    {
                        var ticket = ObtenerNumeroDeTicketDigital(repositorio, aplicaPago, parametros, datosRecorrido, log, pago.IdMOAPay);
                        log.Info($"Número de puesto de trabajo: {parametros.PuestoDeTrabajoId}, Número de ticket: {ticket}.");
                        ImprimirReciboPagoDigital(servicio, documento, datosRecorrido, parametros, aplicaPago, ticket, log, repositorio, pago.Importe.ToString(), pago.Id);
                    }
                }
                else
                {
                    CrearLogActividad(servicio, parametros.WorkflowId, log, "Pago Tasa Municipal Efectivo");
                    log.Info("No es pago Digital, se imprime recibo normal.");
                    var ticket = ObtenerNumeroDeTicket(repositorio, aplicaPago, parametros, datosRecorrido, log);
                    log.Info($"Número de puesto de trabajo: {parametros.PuestoDeTrabajoId}, Número de ticket: {ticket}.");
                    if (!repositorio.TieneContingenciaPorTipo(Constantes.Contingencia.PayCaido))
                        ImprimirRecibo(servicio, documento, datosRecorrido, parametros, aplicaPago, ticket, log, repositorio);
                }
                CrearLogActividad(servicio, parametros.WorkflowId, log, "Impresion Recibo Municipal");
                FinalizarActividad(servicio, parametros.WorkflowId, parametros.PuestoDeTrabajoId, log);
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
                log.Error(e, "Error al ejecutar la actividad de impresión de recibo municipal.");
            }
            return resultado;
        }

        private ParametrosDeImpresion ObtenerParametros(CodeActivityContext context)
        {
            var centroId = CentroId.Get<int>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var workflowId = WorkflowId.Get<Guid>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;

            if (centroId <= 0) throw new ArgumentOutOfRangeException("CentroId");
            if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("Código inválido", "CodigoDeImpresion");
            if (workflowId == Guid.Empty) throw new ArgumentException("WorkflowId vacío", "WorkflowId");
            if (puestoDeTrabajoId <= 0) throw new ArgumentOutOfRangeException("PuestoDeTrabajoId");

            return new ParametrosDeImpresion
            {
                CentroId = centroId,
                Codigo = codigo,
                WorkflowId = workflowId,
                PuestoDeTrabajoId = puestoDeTrabajoId,
                CantCopias = cantCopias
            };
        }

        private void CrearLogActividad(IServicioComandos servicio, Guid workflowId, ILogger log, string actividad)
        {
            var logActividad = new LogActividadDto
            {
                Actividad = actividad,
                ActividadXaml = "ImpresionReciboMunicipal",
                WorkflowInstanceId = workflowId,
                Fecha = DateTime.Now
            };

            var resultado = servicio.Ejecutar(new CrearLogActividad { Dto = logActividad });
            
            if (!resultado.HayErrores)
                log.Debug("Log de actividad creado correctamente.");
        }

        private DocumentoDeImpresionPorCentroDto ObtenerDocumentoDeImpresion(IServicioRepositorio repo, ParametrosDeImpresion parametros, ILogger log)
        {
            var doc = repo.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(parametros.Codigo, parametros.CentroId, parametros.PuestoDeTrabajoId);
            if (doc == null) throw new Exception($"No se encontró el documento de impresión para el código {parametros.Codigo}.");
            log.Debug($"Documento obtenido: {doc.Id} - {doc.DocumentoDeImpresionDescripcion}");
            return doc;
        }

        private RecorridoDto ObtenerYValidarRecorrido(IServicioRepositorio repo, Guid workflowId, ILogger log)
        {
            var recorrido = repo.ObtenerRecorridoPorGuid(workflowId);
            
            if (recorrido?.Workflow == null || string.IsNullOrEmpty(recorrido.Patente) || recorrido.Id <= 0)
                throw new Exception("Recorrido inválido.");
            
            if (recorrido.Material == null)
                throw new Exception("Material inválido en el recorrido.");
            
            log.Info($"Recorrido OK: Id {recorrido.Id}, Patente {recorrido.Patente}");
            
            return recorrido;
        }

        /// <summary>
        /// Valida si aplica el pago de la tasa municipal o si está exceptuado según si es ACA o si ya existe un pago realizado 
        /// durante el día o si cargaron una excepción.
        /// </summary>
        /// <returns>true si aplica pago, false si está exceptuado de pagar</returns>
        private bool DeterminarSiAplicaPago(IServicioRepositorio servicioRepositorio, RecorridoDto recorrido, ParametrosDeImpresion parametros)
        {
            bool aplicaPago = !this.DeterminarSiEsACA(servicioRepositorio, recorrido);

            if (aplicaPago)
                aplicaPago = !this.DeterminarSiExistePagoRealizadoEnElDia(servicioRepositorio, recorrido);

            if (aplicaPago)
            {
                parametros.TieneExcepcion = this.DeterminarSiTieneExcepcionDePagoDeTasaMunicipal(servicioRepositorio, recorrido);
                aplicaPago = !parametros.TieneExcepcion;
            }

            return aplicaPago;
        }

        private bool DeterminarSiEsACA(IServicioRepositorio servicioRepositorio, RecorridoDto recorrido)
        {
            var cartaPorte = servicioRepositorio.ObtenerCartaDePortePorrecorrido(recorrido.Id);
            bool esIngresoImportacion = recorrido.Workflow.Codigo == Constantes.WorkFlow.workflowIngresoImportacion;
            bool esTitularACA = cartaPorte?.TitularCartaPorteCodigoSap == Constantes.ValoresPorDefecto.CodigoSapACA;
            bool esEstablecimientoACA = cartaPorte?.CodEstab == Constantes.ValoresPorDefecto.EstablecimientoACA;

            return esIngresoImportacion && esTitularACA && esEstablecimientoACA;
        }

        private bool DeterminarSiExistePagoRealizadoEnElDia(IServicioRepositorio servicioRepositorio, RecorridoDto recorrido)
        {
            var config = servicioRepositorio.ObtenerConfiguracionGeneral(
                Constantes.ConfiguracionGeneral.ImpresionReciboMunicipal.Actividad,
                Constantes.ConfiguracionGeneral.ImpresionReciboMunicipal.MaterialesPagoRealizado
            );

            var materialesConPago = config?.Valor?.Split(',').Select(x => x.Trim()).ToList() ?? new List<string>();

            return
                materialesConPago.Contains(recorrido.Material.CodigoSAP) &&
                servicioRepositorio.ExistePagoRealizado(recorrido.Patente);
        }

        private bool DeterminarSiTieneExcepcionDePagoDeTasaMunicipal(IServicioRepositorio servicioRepositorio, RecorridoDto recorrido)
        {
            return servicioRepositorio.TieneExcepcionDePagoDeTasaMunicipal(recorrido.Patente, recorrido.InstanciaWorkflow);
        }

        private string ObtenerNumeroDeTicket(IServicioRepositorio repo, bool aplicaPago, ParametrosDeImpresion parametros, ImpresionReciboMunicipalRecorridoDto recorrido, ILogger log)
        {
            log.Debug($"Obteniendo número de ticket para la Patente: {recorrido.Patente}, aplica pago: {aplicaPago}.");
            if (!aplicaPago) 
                return parametros.TieneExcepcion ? "Tasa abonada por excepción" : "Tasa abonada dentro del día";

            var numGarita = 
                repo.ObtenerNumGaritaEntrada(parametros.PuestoDeTrabajoId)?.PadLeft(4, '0') ?? 
                throw new Exception($"Garita no encontrada para puesto {parametros.PuestoDeTrabajoId}");

            var ticket = repo.ObtenerNumeroDeTicketGenerado(parametros.PuestoDeTrabajoId, recorrido.PagoConMercadoPago).ToString().PadLeft(7, '0');
            return $"1{numGarita.Substring(1)}-{$"{ticket}"}";
        }

        private string ObtenerNumeroDeTicketDigital(IServicioRepositorio repo, bool aplicaPago, ParametrosDeImpresion parametros, ImpresionReciboMunicipalRecorridoDto recorrido, ILogger log, int idPago)
        {
            log.Debug($"Obteniendo número de ticket para la Patente: {recorrido.Patente}, aplica pago: {aplicaPago}.");
            if (!aplicaPago) 
                return parametros.TieneExcepcion ? "Tasa abonada por excepción" : "Tasa abonada dentro del día";

            var numGarita = 
                repo.ObtenerNumGaritaEntrada(parametros.PuestoDeTrabajoId)?.PadLeft(4, '0') ?? 
                throw new Exception($"Garita no encontrada para puesto {parametros.PuestoDeTrabajoId}");

            var ticket = idPago.ToString().PadLeft(7, '0');
            return $"2{numGarita.Substring(1)}-{$"{ticket}"}";
        }

        private void ImprimirRecibo(IServicioComandos servicio, DocumentoDeImpresionPorCentroDto documento, ImpresionReciboMunicipalRecorridoDto recorrido, ParametrosDeImpresion parametros, bool aplicaPago, string ticket, ILogger log, IServicioRepositorio repositorio)
        {
            var dto = new ImpReciboMunicipalDto
            {
                Impresora = documento.ImpresoraDireccion ?? "",
                Codigo = parametros.Codigo,
                TicketNro = ticket,
                Ordenanza = recorrido.Ordenanza,
                Valor = aplicaPago ? recorrido.Monto : "0",
                FechaImpresion = DateTime.Now,
                WorkflowId = parametros.WorkflowId,
                Patente = recorrido.Patente,
                TipoVehiculo = recorrido.TipoVehiculo,
                NroDocumentoLegal = ObtenerDocumentoLegal(recorrido)
            };
            log.Debug($"Imprimiendo {parametros.CantCopias} copia(s).");
            var resultado = servicio.Ejecutar(new ImprimirReciboMunicipal { Dto = dto, CantidadCopias = parametros.CantCopias });
            if (resultado.HayErrores)
                throw new Exception("Error al imprimir recibo: " + string.Join(", ", resultado.Errores.Select(e => $"{e.Key}: {e.Value}")));
        }

        private void ImprimirReciboPagoDigital(IServicioComandos servicio, DocumentoDeImpresionPorCentroDto documento, ImpresionReciboMunicipalRecorridoDto recorrido, ParametrosDeImpresion parametros, bool aplicaPago, string ticket, ILogger log, IServicioRepositorio repositorio, string monto, int idPago)
        {
            var dto = new ImpReciboMunicipalDto
            {
                Impresora = documento.ImpresoraDireccion ?? "",
                Codigo = parametros.Codigo,
                TicketNro = ticket,
                Ordenanza = recorrido.Ordenanza,
                Valor = aplicaPago ? monto : "0",
                FechaImpresion = DateTime.Now,
                WorkflowId = parametros.WorkflowId,
                Patente = recorrido.Patente,
                TipoVehiculo = recorrido.TipoVehiculo,
                NroDocumentoLegal = ObtenerDocumentoLegal(recorrido)
            };
            log.Debug($"Imprimiendo {parametros.CantCopias} copia(s).");
            var resultado = servicio.Ejecutar(new ImprimirReciboMunicipal { Dto = dto, CantidadCopias = parametros.CantCopias, IdPagoDigital = idPago });
            if (resultado.HayErrores)
                throw new Exception("Error al imprimir recibo: " + string.Join(", ", resultado.Errores.Select(e => $"{e.Key}: {e.Value}")));
        }

        private string ObtenerDocumentoLegal(ImpresionReciboMunicipalRecorridoDto recorrido)
        {
            if (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito)
            {
                return recorrido.DocumentoInternoSap ?? "";
            }
            if (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte ||
                recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenDeDescargaFason)
            {
                return recorrido.NumeroDocumentoIngreso ?? "";
            }
            return "";
        }

        private void FinalizarActividad(IServicioComandos servicio, Guid workflowId, int puestoId, ILogger log)
        {
            var resultado = servicio.Ejecutar(new FinDeActividad
            {
                InstanceId = workflowId,
                Actividad = "ImpresionReciboMunicipal",
                PuestoDeTrabajoId = puestoId
            });
            if (resultado.HayErrores)
                throw new Exception("Error al finalizar actividad: " + string.Join(", ", resultado.Errores.Select(e => $"{e.Key}: {e.Value}")));
        }

        private List<PagosTasaMunicipal> ObtenerPagosDigitales(IServicioRepositorio repo, Guid workflowId)
        {
            var pagos = repo.ObtenerPagosDigitalesPorInstanceId(workflowId).ToList();
            return pagos;
        }
    }
    public class ParametrosDeImpresion
    {
        public int CentroId { get; set; }
        public string Codigo { get; set; }
        public Guid WorkflowId { get; set; }
        public int PuestoDeTrabajoId { get; set; }
        public int CantCopias { get; set; }
        public bool TieneExcepcion { get; set; }
    }
}