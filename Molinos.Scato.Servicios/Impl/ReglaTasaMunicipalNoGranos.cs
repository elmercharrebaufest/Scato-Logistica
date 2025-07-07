using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Molinos.Scato.Servicios.Impl
{
    public class ReglaTasaMunicipalNoGranos : IReglaTasaMunicipal
    {
        private readonly IRepositorio repositorio;
        private readonly IServicioComandos servicioComandos;
        private readonly ILogger logger;
        private readonly IServicioRepositorio servicioRepositorio;
        public ReglaTasaMunicipalNoGranos(IRepositorio repositorio, IServicioComandos servicioComandos, ILogger logger, IServicioRepositorio servicioRepositorio)
        {
            this.repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            this.servicioComandos = servicioComandos ?? throw new ArgumentNullException(nameof(servicioComandos));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.servicioRepositorio = servicioRepositorio;
        }
        public bool Aplica(DatosTasaMunicipal datos)
        {
            return datos.TipoMaterial == TipoMaterial.NoGranos;
        }

        public TipoVehiculo ObtenerTipoVehiculo(DatosTasaMunicipal datos)
        {
            TipoVehiculo tipoVehiculo;
            if (datos.TipoOrigenDeValidacion == TipoOrigenDeValidacion.CargaDeCupo)
            {
                tipoVehiculo = ConsultarTipoVehiculo(datos.Patente, datos.CentroId, datos.PatenteAcoplado);
            }
            else 
            {
                if((!datos.TipoVehiculo.HasValue))
                    throw new ArgumentException("El tipo de vehículo no puede ser nulo.", nameof(datos.TipoVehiculo));
                else
                    tipoVehiculo = datos.TipoVehiculo.Value;
            }
            return tipoVehiculo;
        }

        public Dictionary<int, TipoValidacionPagoTasaMunicipal> ObtenerPago(DatosTasaMunicipal datos, TipoCategoriaVehiculo tipoCategoria, int numeroDiasDesde)
        {
            logger.Info($"Obteniendo pago de tasa municipal para Patente: {datos.Patente}");
            TipoValidacionPagoTasaMunicipal tipoValidacion;

            string numeroDocumento = string.IsNullOrWhiteSpace(datos.Ctg) ? string.Empty : datos.Ctg;

            var idPago = repositorio.ObtenerIdPagoTasaMunicipal(tipoCategoria, datos.Patente, numeroDocumento, numeroDiasDesde, datos.CentroId, Constantes.MOAPay.Codigos.CodigoDiferenciaDePago);
            logger.Debug($"IdPago obtenido: {idPago} para Patente: {datos.Patente}, Documento: {numeroDocumento}, CentroId: {datos.CentroId}");
            if (idPago > 0)
                tipoValidacion = TipoValidacionPagoTasaMunicipal.Abonado;
            else
            {
                var idPagoCondiferencia = repositorio.ObtenerIdPagoTasaMunicipal(tipoCategoria, datos.Patente, numeroDocumento, numeroDiasDesde, datos.CentroId, Constantes.MOAPay.Codigos.CodigoDiferenciaDePago, true);
                if (idPagoCondiferencia == 0)
                {
                    logger.Debug($"No se encontró un pago válido para Patente: {datos.Patente}, Documento: {numeroDocumento}, CentroId: {datos.CentroId}");
                    tipoValidacion = TipoValidacionPagoTasaMunicipal.Adeudado;
                }
                else
                {
                    logger.Debug($"IdPago con diferencia obtenido: {idPagoCondiferencia} para Patente: {datos.Patente}, Documento: {numeroDocumento}, CentroId: {datos.CentroId}");
                    idPago = idPagoCondiferencia;
                    tipoValidacion = TipoValidacionPagoTasaMunicipal.DiferenciaDePago;
                }
            }
            return new Dictionary<int, TipoValidacionPagoTasaMunicipal> { { idPago, tipoValidacion } };
        }

        private TipoVehiculo ConsultarTipoVehiculo(string patente, int centroId, string patenteAcoplado = null)
        {
            logger.Info($"Consultando tipo de vehículo para Patente: {patente}, Patente Acoplado: {patenteAcoplado}, CentroId: {centroId}");
            var comandoCNRT = GenerarComandoConsultarCNRT(patente, patenteAcoplado);

            logger.Info($"Comando generado: {comandoCNRT.ToJson()}");
            var resultado = servicioComandos.Ejecutar(comandoCNRT) as ResultadoEscalables;

            if (resultado == null)
                throw new InvalidOperationException("Error en CNRT: Resultado nulo");

            if (resultado.HayErrores)
            {
                var errores = string.Join(", ", resultado.Errores.Select(e => e.Value));
                throw new InvalidOperationException($"Error en CNRT: {errores}");
            }

            if (!resultado.Categoria.HasValue)
                throw new InvalidOperationException($"Error en CNRT: {Textos.CategoriaEscalable_Nula}");

            logger.Info($"Tipo de vehículo encontrado: {resultado.Categoria.Value} para Patente: {patente}, Patente Acoplado: {patenteAcoplado}");
            return resultado.Categoria.Value;
        }

        private Comando GenerarComandoConsultarCNRT(string patente, string patenteAcoplado)
        {
            logger.Info($"Generando comando para consultar CNRT con Patente: {patente}, Patente Acoplado: {patenteAcoplado}");
            var config = repositorio.Obtener<ConfiguracionGeneral>(x =>
                x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.IngresarOrdenCargaInternaFason &&
                x.Nombre == Constantes.ConfiguracionGeneral.CNRT.CNRTDummy &&
                x.CentroId == null);

            var esDummy = false;

            if (!string.IsNullOrEmpty(config?.Valor))
                esDummy = bool.TryParse(config.Valor, out var dummyActivo) && dummyActivo;

            return esDummy ?
             new ConsultarEscalablesDummy { Patente = patente, Acoplado = patenteAcoplado } as Comando :
             new ConsultarEscalables { Patente = patente, Acoplado = patenteAcoplado } as Comando;
        }
    }
}
