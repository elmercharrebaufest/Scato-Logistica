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
using System.Linq.Expressions;

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

                var pago = regla.ObtenerPago(datos, categoriaVehiculo, numeroDiasParaInicioBusqueda).FirstOrDefault();
                if (pago.Key == 0)
                {
                    Log.Info($"No se encontró un pago asociado a la patente: {datos.Patente} en el centro: {datos.CentroId}");
                    builder.AsignarValidacionDePago(pago.Value);
                    return builder.ConstruirResultado();
                }

                int idPago = pago.Key;
                var tipoValidacionPago = pago.Value;
                var validacion = ValidarPago(idPago, tipoValidacionPago, datos, regla, builder);
                Log.Debug($"Validación del pago: {validacion} para el pago con ID: {idPago}");

                builder.AsignarValidacionDePago(validacion);
                if (validacion == TipoValidacionPagoTasaMunicipal.Abonado)
                {
                    Log.Info($"Aplica actualizacion interna: {datos.AplicaActualizacionInterna}");
                    if (datos.AplicaActualizacionInterna)
                    {
                        Log.Debug($"Actualizando estado del pago con ID: {idPago} y InstanceId: {datos.InstanceId}, IdDiferenciaPago: {builder.Resultado.IdDiferenciaDePago}");
                        ActualizarEstadoDePago(idPago, datos.InstanceId, builder.Resultado.IdDiferenciaDePago ?? 0);
                        InformarPago(builder.Resultado.IdPay);
                        if (builder.Resultado.IdPayComplemento > 0)
                            InformarPago(builder.Resultado.IdPayComplemento);

                    }
                    else
                    {
                        builder.AsignarIdDePago(idPago);
                        builder.AsignarIdDiferenciaDePago(builder.Resultado.IdDiferenciaDePago ?? 0);
                    }

                    Log.Info($"Pago abonado correctamente para el pago con ID: {idPago}");
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
        private TipoValidacionPagoTasaMunicipal ValidarPago(int idPago, TipoValidacionPagoTasaMunicipal tipoValidacion, DatosTasaMunicipal datosTasa, IReglaTasaMunicipal regla, PagoTasaMunicipalBuilder builder)
        {
            var pagoMunicipal = Repositorio.Obtener<PagosTasaMunicipal>(p => p.Id == idPago);
            builder.AsignarIdPay(pagoMunicipal.IdMOAPay);

            if (tipoValidacion == TipoValidacionPagoTasaMunicipal.DiferenciaDePago)
            {
                var diferenciaDePago = ObtenerDiferenciaDePago(datosTasa, pagoMunicipal.NumeroDocumento);
                if (diferenciaDePago == null)
                {
                    Log.Info($"No se encontró una pago para la diferencia de pago para el pago con ID: {idPago} y monto: {pagoMunicipal.Importe}");
                    return tipoValidacion;
                }
                var montoPago = pagoMunicipal.Importe ?? 0;
                var sumaDePagos = montoPago + (diferenciaDePago.Importe ?? 0);
                var recibo = Repositorio.ObtenerMayor<ReciboMunicipal, DateTime>(r => r.Centro.Id == datosTasa.CentroId && r.TipoVehiculo == null && r.FechaActivacion <= diferenciaDePago.FechaPago, o => o.FechaActivacion);

                if (sumaDePagos >= recibo.Monto)
                {
                    Log.Info($"Pago con ID: {idPago} y monto: {montoPago} más diferencia de pago: {diferenciaDePago.Importe ?? 0} es suficiente para cubrir el recibo con monto: {recibo}");
                    builder.AsignarIdDiferenciaDePago(diferenciaDePago.Id);
                    builder.AsignarIdPayComplemento(diferenciaDePago.IdMOAPay);
                    return TipoValidacionPagoTasaMunicipal.Abonado;
                }
            }
            return tipoValidacion;
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

        private PagosTasaMunicipal ObtenerDiferenciaDePago(DatosTasaMunicipal datos, string numeroDeDocumento)
        {
            Log.Info($"Obteniendo diferencia de pago de tasa municipal para Patente: {datos.Patente} y Documento: {numeroDeDocumento}");
            string documento = numeroDeDocumento.Trim() + "DP";
            Expression<Func<PagosTasaMunicipal, bool>> filtro = c => c.Disponible &&
                                                                     c.Dominio == datos.Patente &&
                                                                     c.NumeroDocumento == documento;

            var pago = Repositorio.ObtenerMayor<PagosTasaMunicipal, DateTime>(filtro, o => o.FechaPago ?? DateTime.Today);
            return pago;
        }
    }
}