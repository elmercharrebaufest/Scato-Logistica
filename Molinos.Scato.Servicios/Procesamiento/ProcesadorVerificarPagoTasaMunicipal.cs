using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
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
        private readonly List<IReglaExcepcionTasaMunicipal> _reglasExcepcion;
        private readonly List<IReglaPago24HrsTasaMunicipal> _reglasPago24Hrs;

        public ProcesadorVerificarPagoTasaMunicipal(IRepositorio repositorio, IConversor conversor,
                                                    ILogger log, IServicioComandos servicioComandos,
                                                    IServicioRepositorio servicioRepositorio, List<IReglaTasaMunicipal> reglas,
                                                    ICategorizadorVehiculo categorizador, List<IReglaExcepcionTasaMunicipal> reglasExcepcion,
                                                    List<IReglaPago24HrsTasaMunicipal> reglasPago24Hrs)
            : base(repositorio, conversor, log)
        {
            _servicioComandos = servicioComandos;
            _servicioRepositorio = servicioRepositorio;
            _reglas = reglas;
            _categorizador = categorizador;
            _reglasExcepcion = reglasExcepcion;
            _reglasPago24Hrs = reglasPago24Hrs;
        }

        public override Resultado Ejecutar(VerificarPagoTasaMunicipal comando)
        {
            Log.Info($"Ejecutando procesamiento para {typeof(VerificarPagoTasaMunicipal).Name}");
            PagoTasaMunicipalBuilder builder = new PagoTasaMunicipalBuilder();
            try
            {
                Validar(comando);
                Log.Info($"Validación inicial del comando {comando.Patente}, {comando.MaterialId}, {comando.CodigoEstablecimiento}, {comando.Ctg}  completada con éxito.");
                var configuracion = _servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.PagoTasaMunicipal, Constantes.ConfiguracionGeneral.PagoTasaMunicipal.NumeroDiasParaInicioBusqueda);
                int.TryParse(configuracion?.Valor, out int numeroDiasParaInicioBusqueda);
                Log.Info($"Número de días para inicio de búsqueda: {numeroDiasParaInicioBusqueda}");

                bool tieneExcepcion = ValidarExcepciones(comando, builder);

                if (tieneExcepcion)
                {
                    Log.Info($"Excepción encontrada para el comando: {JsonConvert.SerializeObject(comando)}");
                    return builder.ConstruirResultado();
                }

                bool tienePago24Hrs = ValidarPago24Hrs(comando, builder);
                if (tienePago24Hrs)
                {
                    Log.Info($"Pago de 24 horas encontrado para el comando: {JsonConvert.SerializeObject(comando)}");
                    return builder.ConstruirResultado();
                }

                if (ValidadoAlIngreso(comando, builder))
                {
                    return builder.ConstruirResultado();
                }

                var datos = Conversor.Convertir<VerificarPagoTasaMunicipal, DatosTasaMunicipal>(comando);

                var regla = _reglas.FirstOrDefault(r => r.Aplica(datos));

                var tipoVehiculo = regla.ObtenerTipoVehiculo(datos);
                Log.Debug($"Tipo de vehículo obtenido: {tipoVehiculo} para la patente: {datos.Patente} y centro: {datos.CentroId}");

                var categoriaVehiculo = _categorizador.ObtenerCategoria(tipoVehiculo);
                Log.Debug($"Categoría de vehículo obtenida: {categoriaVehiculo} para la patente: {datos.Patente} y centro: {datos.CentroId}");

                var pago = ObtenerPago(datos, categoriaVehiculo, numeroDiasParaInicioBusqueda);
                ValidarPago(datos.Patente, pago, builder);

               if(builder.ObtenerCondicionDePago() == TipoValidacionPagoTasaMunicipal.Abonado)
               {
                    Log.Info($"Aplica actualizacion interna: {datos.AplicaActualizacionInterna}");
                    if (datos.AplicaActualizacionInterna)
                    {
                        Log.Debug($"Actualizando estado del pago con ID: {pago.IdPagoNormal} y InstanceId: {datos.InstanceId}, IdDiferenciaPago: {pago.IdDiferenciaPago}");
                        ActualizarEstadoDePago(pago.IdPagoNormal, datos.InstanceId, pago.IdDiferenciaPago ?? 0);
                        InformarPago(pago.IdPayPagoNormal);
                        if (pago.IdPayDiferenciaPago.HasValue && pago.IdPayDiferenciaPago.Value > 0)
                            InformarPago(pago.IdPayDiferenciaPago.Value);
                    }
                    else
                    {
                        builder.AsignarIdDePago(pago.IdPagoNormal);
                        builder.AsignarIdDiferenciaDePago(pago.IdDiferenciaPago ?? 0);
                    }
                    Log.Info($"Pago abonado correctamente para la Patente: {pago.Dominio}, pago con Id: {pago.IdPagoNormal}, pago con Id diferencia: {pago.IdDiferenciaPago}");
               }
            }
            catch (Exception ex)
            {
                Log.Error($"Error al procesar el comando {typeof(VerificarPagoTasaMunicipal).Name}: {ex.Message}", ex);
                builder.AsignarError("Error", ex.Message);
                builder.AsignarValidacionDePago(TipoValidacionPagoTasaMunicipal.Error);
                return builder.ConstruirResultado();
            }
            return builder.ConstruirResultado();
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
                throw new InvalidOperationException($"No se pudo actualizar el pago de la tasa municipal. Error en el comando: {nameof(ModificarComoUsadoPagosTasaMunicipal)} - {respuestaModificacionEstadoPago.Errores}");

            Log.Info($"Estado del pago actualizado exitosamente");
            return respuestaModificacionEstadoPago;
        }

        private Resultado InformarPago(int idPago)
        {
            Log.Info($"Informar con IdPago: {idPago}");
            var respuestaInformarPago = _servicioComandos.Ejecutar(new MOAPayInformarPagoComoConsumido
            {
                Id = idPago,
                Disponible = "N"
            });

            if (respuestaInformarPago == null)
                throw new InvalidOperationException("No se pudo obtener el resultado al informar el pago.");

            if (respuestaInformarPago.HayErrores)
                throw new InvalidOperationException($"No se pudo informar el pago de la tasa municipal.");

            return respuestaInformarPago;
        }

        private bool ValidarExcepciones(VerificarPagoTasaMunicipal comando, PagoTasaMunicipalBuilder builder)
        {
            var datosExcepcion = Conversor.Convertir<VerificarPagoTasaMunicipal, DatosExcepcionTasaMunicipal>(comando);
            var reglaExcepcion = _reglasExcepcion.FirstOrDefault(r => r.Aplica(datosExcepcion));
            if (reglaExcepcion != null)
            {
                var validacionExcepcion = reglaExcepcion.ValidarExcepcion(datosExcepcion).FirstOrDefault();

                if (validacionExcepcion.Value)
                {
                    builder.AsignarValidacionDePago(validacionExcepcion.Key);

                }
                return validacionExcepcion.Value;
            }
            else
                return false;
        }

        private bool ValidarPago24Hrs(VerificarPagoTasaMunicipal comando, PagoTasaMunicipalBuilder builder)
        {
            var datosExcepcion = Conversor.Convertir<VerificarPagoTasaMunicipal, DatosExcepcionTasaMunicipal>(comando);
            var reglaExcepcion = _reglasPago24Hrs.FirstOrDefault(r => r.Aplica(datosExcepcion));
            if (reglaExcepcion != null)
            {
                var validacionExcepcion = reglaExcepcion.ValidarExcepcion(datosExcepcion).FirstOrDefault();

                if (validacionExcepcion.Value)
                {
                    builder.AsignarValidacionDePago(validacionExcepcion.Key);

                }
                return validacionExcepcion.Value;
            }
            else
                return false;

        }

        private bool ValidadoAlIngreso(VerificarPagoTasaMunicipal comando, PagoTasaMunicipalBuilder builder)
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
                builder.AsignarValidacionDePago(TipoValidacionPagoTasaMunicipal.Abonado);
                InformarPago(pago.IdMOAPay);
                string numeroDocumento = pago.NumeroDocumento + Constantes.MOAPay.Codigos.CodigoDiferenciaDePago;
                var complementoPago = Repositorio.Obtener<PagosTasaMunicipal>(p => p.NumeroDocumento == numeroDocumento && p.IdInstance == comando.InstanceId.Value && !p.Disponible);
                if (complementoPago != null && complementoPago.IdMOAPay > 0)
                    InformarPago(complementoPago.IdMOAPay);
            }
            return validado;
        }

        public void ValidarPago(string patente, EstadoPagoTasaMunicipal pago, PagoTasaMunicipalBuilder builder)
        {
            TipoValidacionPagoTasaMunicipal tipoValidacion;
            switch (pago.CondicionDePago)
            {
                case TipoValidacionPagoTasaMunicipal.Abonado:
                    Log.Debug($"Pago normal encontrado para la patente: {patente}, documento: {pago.NumeroDocumento}, importe: {pago.ImporteNormal}");
                    builder.AsignarIdDePago(pago.IdPagoNormal);
                    builder.AsignarIdPay(pago.IdPayPagoNormal);
                    if (pago.IdDiferenciaPago.HasValue && pago.IdDiferenciaPago.Value > 0)
                    {
                        Log.Debug($"Pago de diferencia encontrado para la patente: {patente}, documento: {pago.NumeroDocumento}, importe DP: {pago.ImporteDP}, total pagado {pago.TotalPagado}");
                        builder.AsignarIdDiferenciaDePago(pago.IdDiferenciaPago.Value);
                        builder.AsignarIdPayComplemento(pago.IdPayDiferenciaPago.Value);
                    }
                    else
                    {
                        Log.Debug($"No se encontró un pago de diferencia para la patente: {patente} y documento: {pago.NumeroDocumento}");
                    }
                    tipoValidacion = TipoValidacionPagoTasaMunicipal.Abonado;
                    break;

                case TipoValidacionPagoTasaMunicipal.DiferenciaDePago:
                    Log.Debug($"Pago con diferencia encontrado para la patente: {patente}, documento: {pago.NumeroDocumento}, importe: {pago.ImporteNormal}, importe requerido: {pago.TarifaTipoVehiculo}");
                    tipoValidacion = TipoValidacionPagoTasaMunicipal.DiferenciaDePago;
                    break;

                default:
                    Log.Warn($"Pago no encontrado para la patente: {patente}, documento: {pago.NumeroDocumento}");
                    tipoValidacion = TipoValidacionPagoTasaMunicipal.Adeudado;
                    break;
            }
            Log.Debug($"Validación del pago: {tipoValidacion} para patente: {patente} y pago con ID: {pago.IdPagoNormal}");
            builder.AsignarValidacionDePago(tipoValidacion);
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
    }
}