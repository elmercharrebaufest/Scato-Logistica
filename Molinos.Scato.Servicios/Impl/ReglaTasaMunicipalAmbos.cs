using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.Scato.Servicios.Impl
{
    public class ReglaTasaMunicipalAmbos : IReglaTasaMunicipal
    {
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;

        public ReglaTasaMunicipalAmbos(IServicioRepositorio servicioRepositorio, ILogger logger, IRepositorio repositorio)
        {
            this.servicioRepositorio = servicioRepositorio ?? throw new ArgumentNullException(nameof(servicioRepositorio));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repositorio = repositorio;
        }
        public bool Aplica(DatosTasaMunicipal datos)
        {
            return datos.TipoOrigenDeValidacion == TipoOrigenDeValidacion.FormularioWorkflow || datos.TipoOrigenDeValidacion == TipoOrigenDeValidacion.Recorrido;
        }
        public TipoVehiculo ObtenerTipoVehiculo(DatosTasaMunicipal datos)
        {
           if(!datos.TipoVehiculo.HasValue)
            {
                throw new ArgumentException("El tipo de vehículo no puede ser nulo.", nameof(datos.TipoVehiculo));
            }
            return datos.TipoVehiculo.Value;
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

    }

    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var leftVisitor = new ReplaceParameterVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left, right), parameter);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression oldParameter;
            private readonly ParameterExpression newParameter;

            public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                this.oldParameter = oldParameter;
                this.newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == oldParameter ? newParameter : base.VisitParameter(node);
            }
        }
    }
}
