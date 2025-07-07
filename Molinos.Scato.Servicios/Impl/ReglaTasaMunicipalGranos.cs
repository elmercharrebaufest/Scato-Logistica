using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Molinos.Scato.Servicios.Impl
{
    public class ReglaTasaMunicipalGranos:IReglaTasaMunicipal
    {
        private readonly IRepositorio repositorio;
        private readonly IServicioComandos servicioComandos;
        private readonly ILogger logger;
        private readonly IServicioRepositorio servicioRepositorio;

        public ReglaTasaMunicipalGranos(IRepositorio repositorio, IServicioComandos servicioComandos, ILogger logger, IServicioRepositorio servicioRepositorio)
        {
            this.repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            this.servicioComandos = servicioComandos ?? throw new ArgumentNullException(nameof(servicioComandos));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.servicioRepositorio = servicioRepositorio ?? throw new ArgumentNullException(nameof(servicioRepositorio));
        }
        public bool Aplica(DatosTasaMunicipal datos)
        {
            return datos.TipoMaterial == TipoMaterial.Granos;
        }
        public TipoVehiculo ObtenerTipoVehiculo(DatosTasaMunicipal datos)
        {
            TipoVehiculo tipoVehiculo;
            if (datos.TipoOrigenDeValidacion == TipoOrigenDeValidacion.CargaDeCupo) 
            {
                if (string.IsNullOrWhiteSpace(datos.Ctg))
                {
                    throw new ArgumentException("Debe especificar un CTG ", nameof(datos.Ctg));
                }
                  tipoVehiculo = ConsultarTipoVehiculo(datos.Ctg, datos.CentroId);
            }
            else 
            {
                if (!datos.TipoVehiculo.HasValue)
                {
                    throw new ArgumentException("El tipo de vehículo no puede ser nulo.", nameof(datos.TipoVehiculo));
                }

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
                    logger.Warn($"No se encontró un pago válido para Patente: {datos.Patente}, Documento: {numeroDocumento}, CentroId: {datos.CentroId}");
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

        private TipoVehiculo ConsultarTipoVehiculo(string ctg , int centroId)
        {
            logger.Info($"Consultando tipo de vehículo para CTG: {ctg}, CentroId: {centroId}");
            int pesoBruto = ObtenerPesoBrutoPorCTG(ctg, centroId);

            var vehiculo = servicioRepositorio.ObtenerTipodVehiculoPorPesoBruto(pesoBruto, centroId);

            logger.Info($"Tipo de vehículo encontrado: {vehiculo} para peso bruto: {pesoBruto}");
            return vehiculo;
        }
        private int ObtenerPesoBrutoPorCTG(string ctg, int CentroId)
        {
            logger.Info($"Obteniendo peso bruto por CTG: {ctg} y CentroId: {CentroId}");
            int pesoBruto = 0;
            var cartaPorteElectronica = servicioRepositorio.ObtenerCartaPorteElectronicaPorCTG(ctg);

            if (cartaPorteElectronica != null)
                pesoBruto = cartaPorteElectronica.PesoBruto;
            else
            {
                var cartaPorte = servicioComandos.Ejecutar(new ConsultarCPDigital { CentroId = CentroId, NroCtg = long.Parse(ctg) }) as ResultadoCartaPorteElectronica
                               ?? throw new InvalidOperationException($"Error en obtener: {nameof(ResultadoCartaPorteElectronica)}");

                var vehiculo = cartaPorte.Cpe.Vehiculos?.FirstOrDefault()
                                  ?? throw new InvalidOperationException($"Error en obtener: Vehiculos en {nameof(ResultadoCartaPorteElectronica)}");

                logger.Info($"Peso bruto origen obtenido: {vehiculo.PesoBrutoOrigen}");
                pesoBruto = vehiculo.PesoBrutoOrigen.Value;
            }
            return pesoBruto ;

        }
    } 
}
