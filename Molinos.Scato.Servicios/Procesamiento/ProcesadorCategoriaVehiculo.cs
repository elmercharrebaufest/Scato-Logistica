using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public abstract class ProcesadorCategoriaVehiculo<TComando> : ProcesadorComando<TComando>
        where TComando : Comando
    {
        private readonly ICategorizadorVehiculo _categorizador;

        protected ProcesadorCategoriaVehiculo(
            IRepositorio repositorio,
            IConversor conversor,
            ILogger log,
            ICategorizadorVehiculo categorizador)
            : base(repositorio, conversor, log)
        {
            _categorizador = categorizador ?? throw new ArgumentNullException(nameof(categorizador));
        }

        public override Resultado Ejecutar(TComando comando)
        {
            Log.Info($"Iniciando procesamiento para {typeof(TComando).Name}");
            var resultado = new ResultadoConsultarCategoriaVehiculo();
            if (comando == null)
            {
                resultado.Error("Comando", $"El comando: {nameof(comando)} no puede ser nulo.");
                return resultado;
            }

            try
            {
                Log.Info($"Ejecutando procesamiento para {typeof(TComando).Name}");

                Validar(comando, resultado);

                if (resultado.HayErrores)
                {
                    Log.Warn("Validación fallida. No se continuará con el procesamiento.");
                    return resultado;
                }

                var tipoVehiculo = ConsultarTipoVehiculo(comando);
                Log.Info($"Tipo de vehículo consultado: {tipoVehiculo}");
                var categoria = _categorizador.ObtenerCategoria(tipoVehiculo);
                Log.Info($"Categoría asignada: {categoria}");

                if (categoria == TipoCategoriaVehiculo.NoAutomotor)
                    resultado.EsAutomotor = false;
                else
                    resultado.ImporteTasaMunicipal = ObtenerImportePorTipoVehiculo(categoria, IngresarCentroId(comando));
            }
            catch (InvalidOperationException ex)
            {
                Log.Warn(ex, "Categoría no identificada.");
                resultado.Errores.Add(nameof(comando), ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error inesperado en el procesamiento.");
                resultado.Errores.Add("ErrorGeneral", "Ocurrió un error inesperado.");
            }

            return resultado;
        }
        private Dictionary<DateTime, decimal> ObtenerImportePorTipoVehiculo(TipoCategoriaVehiculo tipoCategoria, int centroId)
        {
            Log.Info($"Obteniendo importe por tipo de vehículo: {tipoCategoria} para el centro: {centroId}");
            Expression<Func<ReciboMunicipal, bool>> filtro;

            if (tipoCategoria == TipoCategoriaVehiculo.Comun)
            {
                filtro = c => c.Centro.Id == centroId && c.TipoVehiculo == TipoVehiculo.Camión;
            }
            else
            {
                filtro = c => c.Centro.Id == centroId && c.TipoVehiculo == null;
            }

            var recibo = Repositorio.ObtenerMasReciente<ReciboMunicipal>(filtro, x => x.FechaActivacion);

            if (recibo == null)
                throw new InvalidOperationException("No se encontró un recibo válido para el centro: " + centroId);

            Log.Info($"Recibo encontrado: {recibo?.Id} para el centro: {centroId} con tipo de vehículo: {tipoCategoria} fecha: {recibo.FechaActivacion}");
            var importeConFecha = new Dictionary<DateTime, decimal>(1);
            importeConFecha.Add(recibo.FechaActivacion, recibo.Monto);
            return importeConFecha;
        }
        protected abstract void Validar(TComando comando, Resultado resultado);
        protected abstract TipoVehiculo ConsultarTipoVehiculo(TComando comando);
        protected abstract int IngresarCentroId(TComando comando);


    }
}
