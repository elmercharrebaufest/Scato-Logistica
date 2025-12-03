using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorVerificarPagoTasaMunicipal : ProcesadorComando<VerificarPagoTasaMunicipal>
    {
        private readonly IServicioComandos _servicioComandos;
        private readonly IServicioRepositorio _servicioRepositorio;
        private readonly List<IReglaTasaMunicipal> _reglas;
        private readonly ICategorizadorVehiculo _categorizador;

        public ProcesadorVerificarPagoTasaMunicipal(IRepositorio repositorio, IConversor conversor,
                                                    ILogger log, IServicioComandos servicioComandos,
                                                    IServicioRepositorio servicioRepositorio, List<IReglaTasaMunicipal> reglas,
                                                    ICategorizadorVehiculo categorizador)
            : base(repositorio, conversor, log)
        {
            _servicioComandos = servicioComandos;
            _servicioRepositorio = servicioRepositorio;
            _reglas = reglas;
            _categorizador = categorizador;
        }

        public override Resultado Ejecutar(VerificarPagoTasaMunicipal comando)
        {
            Log.Info($"Ejecutando procesamiento para {typeof(VerificarPagoTasaMunicipal).Name}");
            ResultadoConsultarPagoTasaMunicipal resultado = new ResultadoConsultarPagoTasaMunicipal();
            try
            {
                Validar(comando);
                ObtenerConfiguracionDeIngresoCamion(resultado);
                Log.Info($"Validación inicial del comando {comando.Patente}, {comando.MaterialId}, {comando.CodigoEstablecimiento}, {comando.Ctg}  completada con éxito.");
                var configuracion = _servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.PagoTasaMunicipal, Constantes.ConfiguracionGeneral.PagoTasaMunicipal.NumeroDiasParaInicioBusqueda);
                int.TryParse(configuracion?.Valor, out int numeroDiasParaInicioBusqueda);
                Log.Info($"Número de días para inicio de búsqueda: {numeroDiasParaInicioBusqueda}");

                if (TieneExcepciones(comando, resultado) ||
                    TienePago24Hrs(comando, resultado) ||
                    FueValidadoAlIngreso(comando, resultado))
                {
                    return resultado;
                }

                var datos = Conversor.Convertir<VerificarPagoTasaMunicipal, DatosTasaMunicipal>(comando);
                var regla = _reglas.FirstOrDefault(r => r.Aplica(datos));
                var tipoVehiculo = regla.ObtenerTipoVehiculo(datos);
                Log.Debug($"Tipo de vehículo obtenido: {tipoVehiculo} para la patente: {datos.Patente} y centro: {datos.CentroId}");
                var categoriaVehiculo = _categorizador.ObtenerCategoria(tipoVehiculo);
                Log.Debug($"Categoría de vehículo obtenida: {categoriaVehiculo} para la patente: {datos.Patente} y centro: {datos.CentroId}");
                var pago = ObtenerPago(datos, categoriaVehiculo, numeroDiasParaInicioBusqueda);
                ValidarPago(datos.Patente, pago, datos, resultado, comando.Demorado);
            }
            catch (Exception ex)
            {
                Log.Error($"Error al procesar el comando {typeof(VerificarPagoTasaMunicipal).Name}: {ex.Message}", ex);
                resultado.Errores.Add("Error", ex.Message);
                ConstruirResultado(resultado, TipoValidacionPagoTasaMunicipal.Error, comando.Demorado);
                return resultado;
            }
            return resultado;
        }

        private void Validar(VerificarPagoTasaMunicipal comando)
        {
            if (string.IsNullOrWhiteSpace(comando.Patente) && string.IsNullOrWhiteSpace(comando.PatenteAcoplado))
            {
                throw new ArgumentException("Debe especificar al menos una patente.");
            }
            if (comando.CentroId <= 0)
            {
                throw new ArgumentException("El CentroId debe ser un valor válido.");
            }
        }
       
        private Resultado ActualizarEstadoDePago(int idPago, Guid instanceId, int idDiferenciaPago = 0)
        {
            Log.Info($"Actualizando estado del pago con ID: {idPago} e InstanceId: {instanceId}");
            var respuestaModificacionEstadoPago = _servicioComandos.Ejecutar(new ModificarComoUsadoPagosTasaMunicipal
            {
                PagoId = idPago,
                InstanceId = instanceId,
                DiferenciaPagoId = idDiferenciaPago
            });

            if (respuestaModificacionEstadoPago == null)
                throw new InvalidOperationException("No se pudo obtener el resultado de la actualizacion.");

            if (respuestaModificacionEstadoPago.HayErrores)
            {
                Log.Error($"Error al actualizar el pago de la tasa municipal. Error en el comando: {nameof(ModificarComoUsadoPagosTasaMunicipal)} - {string.Join(",",respuestaModificacionEstadoPago.Errores)}");
                throw new InvalidOperationException($"No se pudo actualizar el pago de la tasa municipal. Error en el comando: {nameof(ModificarComoUsadoPagosTasaMunicipal)} - {respuestaModificacionEstadoPago.Errores}");
            }
            Log.Info($"Estado del pago actualizado exitosamente");
            return respuestaModificacionEstadoPago;
        }

        private Resultado InformarPago(int idPago, Guid intanceId)
        {
            Log.Info($"Informar con IdPago: {idPago}");
            var respuestaInformarPago = _servicioComandos.Ejecutar(new MOAPayInformarPagoComoConsumido
            {
                Id = idPago,
                Disponible = "N",
                IdIntance = intanceId
            });

            if (respuestaInformarPago == null)
                throw new InvalidOperationException("No se pudo obtener el resultado al informar el pago.");

            if (respuestaInformarPago.HayErrores)
            {
                Log.Error($"Error al informar el pago de la tasa municipal. Error en el comando: {nameof(MOAPayInformarPagoComoConsumido)} - {string.Join(",",respuestaInformarPago.Errores)}");
                throw new InvalidOperationException($"No se pudo informar el pago de la tasa municipal.");
            }
            return respuestaInformarPago;
        }

        private bool TieneExcepciones(VerificarPagoTasaMunicipal comando, ResultadoConsultarPagoTasaMunicipal resultado)
        {
            var datosExcepcion = Conversor.Convertir<VerificarPagoTasaMunicipal, DatosExcepcionTasaMunicipal>(comando);
            bool tieneExcepcion = EjecutarValidacionesDeExcepciones(datosExcepcion, resultado);
            if (tieneExcepcion)
            {
                Log.Info($"Excepción encontrada para el comando: {JsonConvert.SerializeObject(comando)}");
                ConstruirResultado(resultado, TipoValidacionPagoTasaMunicipal.Abonado, comando.Demorado);
                // Si existe una excepción, se libera el pago asociado al InstanceId

                var excepcion = _servicioRepositorio.ObtenerExcepcionDeTicketMunicipal(datosExcepcion.Patente, datosExcepcion.InstanceId);
                if (excepcion != null)
                    resultado.IdExcepcion = excepcion.Id;

                if (datosExcepcion.InstanceId.HasValue && datosExcepcion.InstanceId.Value != Guid.Empty)
                {
                    var pago = Repositorio.Obtener<PagosTasaMunicipal>(p => p.IdInstance == datosExcepcion.InstanceId);
                    if (pago != null)
                    {
                        pago.Disponible = true;
                        pago.IdInstance = null;
                    }

                    var recorrido = Repositorio.Obtener<Recorrido>(p => p.InstanciaWorkflow == datosExcepcion.InstanceId);
                    if (recorrido != null)
                        recorrido.PagoTasaMunicipalInformado = true;

                    if (excepcion != null)
                        excepcion.WorkflowInstanceId = datosExcepcion.InstanceId;

                    Repositorio.GuardarCambios();
                }
            }

            return tieneExcepcion;
        }

        private bool TienePago24Hrs(VerificarPagoTasaMunicipal comando, ResultadoConsultarPagoTasaMunicipal resultado)
        {
            var datosExcepcion = Conversor.Convertir<VerificarPagoTasaMunicipal, DatosExcepcionTasaMunicipal>(comando);
            bool tienePago24Hrs = ValidarExcepcionPago24Hrs(datosExcepcion);
            if (tienePago24Hrs)
            {
                Log.Info($"Pago de 24 horas encontrado para el comando: {JsonConvert.SerializeObject(comando)}");
                ConstruirResultado(resultado, TipoValidacionPagoTasaMunicipal.Abonado24Hrs, comando.Demorado);
                var recorrido = Repositorio.Obtener<Recorrido>(p => p.InstanciaWorkflow == datosExcepcion.InstanceId);
                if (recorrido != null)
                {
                    recorrido.PagoTasaMunicipalInformado = true;
                    Repositorio.GuardarCambios();
                }
                resultado.TieneExcepcion = tienePago24Hrs;
                resultado.MotivoExcepcion = Constantes.MOAPay.MotivosDeExcepciones.ExcepcionPorPago24Hrs;
            }
            return tienePago24Hrs;
        }

        private bool FueValidadoAlIngreso(VerificarPagoTasaMunicipal comando, ResultadoConsultarPagoTasaMunicipal resultado)
        {
            bool validado = false;
            PagosTasaMunicipal pago = null;
            if (comando.InstanceId.HasValue && comando.InstanceId.Value != Guid.Empty)
                pago = Repositorio.Obtener<PagosTasaMunicipal>(p => p.IdInstance == comando.InstanceId.Value
                                                                && !p.Disponible
                                                                && !p.NumeroDocumento.EndsWith(Constantes.MOAPay.Codigos.CodigoDiferenciaDePago));

            if (pago != null && pago.IdMOAPay > 0)
            {
                Log.Info($"El pago ya fue validado al ingreso para el InstanceId: {comando.InstanceId}");
                validado = true;
                ConstruirResultado(resultado, TipoValidacionPagoTasaMunicipal.Abonado, comando.Demorado);
                InformarPago(pago.IdMOAPay, comando.InstanceId ?? Guid.Empty);
                string numeroDocumento = pago.NumeroDocumento + Constantes.MOAPay.Codigos.CodigoDiferenciaDePago;
                var complementoPago = Repositorio.Obtener<PagosTasaMunicipal>(p => p.NumeroDocumento == numeroDocumento && p.IdInstance == comando.InstanceId.Value && !p.Disponible);
                if (complementoPago != null && complementoPago.IdMOAPay > 0)
                    InformarPago(complementoPago.IdMOAPay, comando.InstanceId ?? Guid.Empty);
            }
            return validado;
        }

        public void ValidarPago(string patente, EstadoPagoTasaMunicipal pago, DatosTasaMunicipal datos, ResultadoConsultarPagoTasaMunicipal resultado, bool esDemorado)
        {
            TipoValidacionPagoTasaMunicipal tipoValidacion;
            switch (pago.CondicionDePago)
            {
                case TipoValidacionPagoTasaMunicipal.Abonado:
                    Log.Debug($"Pago normal encontrado para la patente: {patente}, documento: {pago.NumeroDocumento}, importe: {pago.ImporteNormal}");
                    resultado.IdPago = pago.IdPagoNormal;
                    resultado.IdPay = pago.IdPayPagoNormal;
                    if (pago.IdDiferenciaPago.HasValue && pago.IdDiferenciaPago.Value > 0)
                    {
                        Log.Debug($"Pago de diferencia encontrado para la patente: {patente}, documento: {pago.NumeroDocumento}, importe DP: {pago.ImporteDP}, total pagado {pago.TotalPagado}");
                        resultado.IdDiferenciaDePago = pago.IdDiferenciaPago.Value;
                        resultado.IdPayComplemento =pago.IdPayDiferenciaPago.Value;
                    }
                    else
                    {
                        Log.Debug($"No se encontró un pago de diferencia para la patente: {patente} y documento: {pago.NumeroDocumento}");
                    }

                    Log.Info($"Aplica actualizacion interna: {datos.AplicaActualizacionInterna}");
                    if (datos.AplicaActualizacionInterna)
                    {
                        Log.Debug($"Actualizando estado del pago con ID: {pago.IdPagoNormal} y InstanceId: {datos.InstanceId}, IdDiferenciaPago: {pago.IdDiferenciaPago}");
                        ActualizarEstadoDePago(pago.IdPagoNormal, datos.InstanceId, pago.IdDiferenciaPago ?? 0);
                        InformarPago(pago.IdPayPagoNormal, datos.InstanceId);
                        if (pago.IdPayDiferenciaPago.HasValue && pago.IdPayDiferenciaPago.Value > 0)
                            InformarPago(pago.IdPayDiferenciaPago.Value, datos.InstanceId);
                    }
                    Log.Info($"Pago abonado correctamente para la Patente: {pago.Dominio}, pago con Id: {pago.IdPagoNormal}, pago con Id diferencia: {pago.IdDiferenciaPago}");
                    tipoValidacion = TipoValidacionPagoTasaMunicipal.Abonado;
                    break;

                case TipoValidacionPagoTasaMunicipal.DiferenciaDePago:
                    Log.Debug($"Pago con diferencia encontrado para la patente: {patente}, documento: {pago.NumeroDocumento}, importe: {pago.ImporteNormal}, importe requerido: {pago.TarifaTipoVehiculo}");
                    tipoValidacion = TipoValidacionPagoTasaMunicipal.DiferenciaDePago;
                    resultado.TieneDiferenciaDePago = true;
                    break;

                default:
                    Log.Warn($"Pago no encontrado para la patente: {patente}, documento: {pago.NumeroDocumento}");
                    if(_servicioRepositorio.TieneContingenciaPorTipo(Constantes.Contingencia.PayCaido))
                    {
                        if(datos.InstanceId != Guid.Empty)
                        {
                            MarcarRecorridoComoIngresoEnContingencia(datos.InstanceId);
                        }
                        Log.Info($"Contingencia activa, se permite el ingreso.");
                        tipoValidacion = TipoValidacionPagoTasaMunicipal.Abonado;
                    }
                    else
                        tipoValidacion = TipoValidacionPagoTasaMunicipal.Adeudado;
                    break;
            }
            Log.Debug($"Validación del pago: {tipoValidacion} para patente: {patente} y pago con ID: {pago.IdPagoNormal}");
            ConstruirResultado(resultado, tipoValidacion, esDemorado);
        }

        private EstadoPagoTasaMunicipal ObtenerPago(DatosTasaMunicipal datos, TipoCategoriaVehiculo tipoCategoria, int numeroDiasDeConsulta)
        {
            Log.Info($"Obteniendo pago de tasa municipal para Patente: {datos.Patente}");
            string numeroDocumento = string.IsNullOrWhiteSpace(datos.Ctg) ? string.Empty : datos.Ctg;
            var pagos = Repositorio.ObtenerPagoTasaMunicipal<EstadoPagoTasaMunicipal>(tipoCategoria, datos.Patente, numeroDocumento, numeroDiasDeConsulta, datos.CentroId, Constantes.MOAPay.Codigos.CodigoDiferenciaDePago);
            Log.Debug($"Pagos encontrados para la patente: {datos.Patente}, total de pagos: {pagos.Count}");
            var pago = pagos.Where(p=> p.CondicionDePago == TipoValidacionPagoTasaMunicipal.Abonado).LastOrDefault();
            if (pago != null)
                return pago;
            else
            {
                Log.Debug($"Buscando pago de diferencia para la patente: {datos.Patente}");
                pago = pagos.Where(p => p.CondicionDePago == TipoValidacionPagoTasaMunicipal.DiferenciaDePago).LastOrDefault();
                if (pago != null)
                {
                    Log.Debug($"Pago de diferencia encontrado para la patente: {datos.Patente}, documento: {pago.NumeroDocumento}, importe: {pago.ImporteDP}");
                    return pago;
                }
                else
                {
                    Log.Warn($"No se encontró un pago abonado ni de diferencia para la patente: {datos.Patente}");
                    pago = new EstadoPagoTasaMunicipal
                    {
                        Dominio = datos.Patente,
                        NumeroDocumento = numeroDocumento,
                        CondicionDePago = TipoValidacionPagoTasaMunicipal.Adeudado,
                        TarifaTipoVehiculo = 0,
                        ImporteNormal = 0,
                        ImporteDP = 0,
                        TotalPagado = 0,
                        IdPagoNormal = 0,
                        IdPayPagoNormal = 0,
                        IdDiferenciaPago = null,
                        IdPayDiferenciaPago = null
                    };
                }
            }
            return pago;
        }

        public bool EjecutarValidacionesDeExcepciones(DatosExcepcionTasaMunicipal datos, ResultadoConsultarPagoTasaMunicipal resultado)
        {
            var reglas = new Dictionary<string, Func<bool>>()
            {
                { Constantes.MOAPay.MotivosDeExcepciones.ExcepcionPorMaterialYCentro , () => ValidarExcepcionMaterialPorCentro(datos) },
                { Constantes.MOAPay.MotivosDeExcepciones.ExcepcionPorPatente , () => ValidarExcepcionPorPatente(datos) },
                { Constantes.MOAPay.MotivosDeExcepciones.ExcepcionPorPatenteYDocumento , () => ValidarExcepcionPorPatenteYDocumento(datos) },
                { Constantes.MOAPay.MotivosDeExcepciones.ExcepcionPorSojaImpo , () => ValidarExcepcionEsSojaImpo(datos) }
            };


            foreach (var regla in reglas)
            {
                if (regla.Value())
                {
                    resultado.TieneExcepcion = true;
                    resultado.MotivoExcepcion = regla.Key;
                    return true;
                }
            }
            return false;
        }

        private bool ValidarExcepcionMaterialPorCentro(DatosExcepcionTasaMunicipal datos)
        {
            bool tieneExcepcion = false;
           if(datos.EsValidacionAlIngreso)
                tieneExcepcion = Repositorio.Existe<MaterialPorCentro>(m => m.Material.Id == datos.MaterialId && m.Centro.Id == datos.CentroId && m.ImprimeReciboMunicipal == false);
           else
            {
                var materialCentro = Repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == datos.InstanceId, x => new { MaterialId = x.Material.Id, CentroId = x.Centro.Id });
                tieneExcepcion = Repositorio.ObtenerProyeccion<MaterialPorCentro, bool>(  x => x.Material.Id == materialCentro.MaterialId && x.Centro.Id == materialCentro.CentroId,
                                                                                       x => !x.ImprimeReciboMunicipal);
            }
           return tieneExcepcion;
        }

        private bool ValidarExcepcionPorPatente(DatosExcepcionTasaMunicipal datos)
        {
            return this._servicioRepositorio.TieneExcepcionDePagoDeTasaMunicipal(datos.Patente, datos.InstanceId);
        }

        private bool ValidarExcepcionPorPatenteYDocumento(DatosExcepcionTasaMunicipal datos)
        {
            bool tieneExcepcion = false;

            if (datos.EsValidacionAlIngreso)
            {
                tieneExcepcion = Repositorio.Existe<LogExceptuadosTicketMunicipal>(r => r.Patente == datos.Patente && r.Material.Id == datos.MaterialId && r.PagaTicketMunicipal == false && r.NumeroDocumentoIngreso == datos.Ctg);
            }
            else
            {
                Log.Info($"Validando excepcion de recorrido para InstanceId: {datos.InstanceId}");
                tieneExcepcion = Repositorio.Existe<LogExceptuadosTicketMunicipal>(x => x.WorkflowInstanceId == datos.InstanceId && x.PagaTicketMunicipal == false);
                Log.Info($"Tiene excepcion de recorrido: {tieneExcepcion} para InstanceId: {datos.InstanceId}");
            }

            return tieneExcepcion;
        }

        private bool ValidarExcepcionEsSojaImpo(DatosExcepcionTasaMunicipal datos)
        {
            bool tieneExcepcion = false;
            
            if (!string.IsNullOrWhiteSpace(datos.Ctg) && !string.IsNullOrWhiteSpace(datos.CodigoEstablecimiento))
                tieneExcepcion = ValidarSojaImpoPorCartaPorteElectronica(datos);
            else if (datos.InstanceId.HasValue && datos.InstanceId.Value != Guid.Empty)
                tieneExcepcion = ValidarSojaImpoConRecorrido(datos);
            
            return tieneExcepcion;
        }

        private bool ValidarSojaImpoConRecorrido(DatosExcepcionTasaMunicipal datos)
        {
            var recorrido = _servicioRepositorio.ObtenerRecorridoPorGuid(datos.InstanceId.Value);
            if (!recorrido.Material.EsGrano)
                return false;

            if (recorrido?.Workflow == null || string.IsNullOrEmpty(recorrido.Patente) || recorrido.Id <= 0)
                throw new Exception("Recorrido inválido.");

            var cartaPorte = _servicioRepositorio.ObtenerCartaDePortePorrecorrido(recorrido.Id);
            if (cartaPorte == null)
                throw new ArgumentNullException(nameof(cartaPorte), "La carta de porte no puede ser nula.");

            return ValidarEsSojaImpo(cartaPorte?.TitularCartaPorteCodigoSap, cartaPorte?.CodEstab);
        }

        private bool ValidarSojaImpoPorCartaPorteElectronica(DatosExcepcionTasaMunicipal datos)
        {
            var cartaPorte = _servicioRepositorio.ObtenerCartaPorteElectronicaPorCTG(datos.Ctg);
            var cuitOrigen = cartaPorte?.CuitOrigen.ToString();
            var codigoSAPtitularCP = _servicioRepositorio.ObtenerProveedorPorCuit(ConvertirCuil(cuitOrigen), new TiposProveedor { PR = true }).CodigoSap;

            return ValidarEsSojaImpo(codigoSAPtitularCP, datos.CodigoEstablecimiento);
        }

        private bool ValidarEsSojaImpo(string titularCartaPorteCodigoSap, string codEstab)
        {
            return 
                !string.IsNullOrEmpty(titularCartaPorteCodigoSap) &&
                (titularCartaPorteCodigoSap == Constantes.ValoresPorDefecto.CodigoSapTPR ||
                 (titularCartaPorteCodigoSap == Constantes.ValoresPorDefecto.CodigoSapACA &&
                  !string.IsNullOrEmpty(codEstab) && codEstab == Constantes.ValoresPorDefecto.EstablecimientoACA));
        }

        private bool ValidarExcepcionPago24Hrs(DatosExcepcionTasaMunicipal datos)
        {
            bool tienePago24Hrs = false;
            
            if (datos.MaterialId.HasValue && datos.MaterialId.Value > 0)
            {
                var material = _servicioRepositorio.ObtenerMaterialPorId(datos.MaterialId.Value);
                if (material != null)
                {
                    tienePago24Hrs = _servicioRepositorio.ExistePagoRealizadoPorListaMaterial(material.CodigoSAP, datos.Patente);
                }
            }

            return tienePago24Hrs;
        }

        private void ConstruirResultado(ResultadoConsultarPagoTasaMunicipal resultado, TipoValidacionPagoTasaMunicipal condicionPago, bool esDemorado)
        {       
            switch (condicionPago)
            {
                case TipoValidacionPagoTasaMunicipal.Abonado:
                    resultado.TipoAlerta = TipoAlerta.Exito;
                    resultado.MensajeAlerta = "TASA ABONADA";
                    resultado.SeLevantaBarrera = true;
                    resultado.EsPagoAbonado = true;
                    resultado.EjecutaWorkFlow = true;
                    break;

                case TipoValidacionPagoTasaMunicipal.Adeudado:
                    resultado.TipoAlerta = TipoAlerta.Error;
                    resultado.MensajeAlerta = resultado.TieneConfiguracionDeBloqueoDeIngreso && !esDemorado ? "TASA ADEUDADA - PARA AVANZAR DEBE DEMORAR EL CAMIÓN" : !resultado.TieneConfiguracionDeBloqueoDeIngreso ? "TASA ADEUDADA" : "TASA ADEUDADA";
                    resultado.SeLevantaBarrera = true;
                    resultado.EsPagoAbonado = false;
                    resultado.EjecutaWorkFlow = resultado.TieneConfiguracionDeBloqueoDeIngreso ? false : true;
                    break;

                case TipoValidacionPagoTasaMunicipal.DiferenciaDePago:
                    resultado.TipoAlerta = TipoAlerta.Exito;
                    resultado.MensajeAlerta = "EXISTEN DIFERENCIAS EN EL PAGO";
                    resultado.SeLevantaBarrera = true;
                    resultado.EsPagoAbonado = false;
                    resultado.EjecutaWorkFlow = false;
                    break;

                case TipoValidacionPagoTasaMunicipal.Abonado24Hrs:
                    resultado.TipoAlerta = TipoAlerta.Exito;
                    resultado.MensajeAlerta = "TASA ABONADA 24 HRS";
                    resultado.SeLevantaBarrera = true;
                    resultado.EsPagoAbonado = true;
                    resultado.EjecutaWorkFlow = true;
                    break;

                default:
                    resultado.TipoAlerta = TipoAlerta.Error;
                    resultado.MensajeAlerta = "ERROR AL CONSULTAR PAGO";
                    resultado.SeLevantaBarrera = true;
                    resultado.EsPagoAbonado = false;

                    break;
            }
        }

        private string ConvertirCuil(string cuil)
        {
            if (String.IsNullOrEmpty(cuil))
            {
                return "";
            }
            if (cuil.Length != 11)
            {
                throw new ArgumentException(Textos.DatoConLongitudIncorrecta);
            }

            string validador1 = cuil.Substring(0, 2);
            string documento = cuil.Substring(2, 8);
            string validador2 = cuil.Substring(10, 1);
            return validador1 + "-" + documento + "-" + validador2;
        }

        private void ObtenerConfiguracionDeIngresoCamion(ResultadoConsultarPagoTasaMunicipal resultado)
        {
            var configuracionIngreso = _servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.PagoTasaMunicipal, Constantes.ConfiguracionGeneral.PagoTasaMunicipal.PermitirBloqueoDeIngreso);
            bool.TryParse(configuracionIngreso?.Valor, out bool permitirBloqueoDeIngreso);
            resultado.TieneConfiguracionDeBloqueoDeIngreso = permitirBloqueoDeIngreso;
        }

        private void MarcarRecorridoComoIngresoEnContingencia(Guid instanceId)
        {
            var recorrido = Repositorio.Obtener<Recorrido>(p => p.InstanciaWorkflow == instanceId);
            if (recorrido != null)
            {
                recorrido.IngresoContingenciaPagoMunicipal = true;
                Repositorio.GuardarCambios();
            }
        }
    }
}