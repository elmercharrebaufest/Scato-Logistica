using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorVisecDistribuirStock : ProcesadorComando<VisecDistribuirStock>
    {
        public ProcesadorVisecDistribuirStock(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(VisecDistribuirStock comando)
        {
            var resultado = new ResultadoVisecDistribuirStock();
            try
            {
                var stocks = Repositorio.Listar<VisecTransmision>(q =>
                    q.Producto == comando.Producto
                    && q.NumeroRUCADestino == comando.RUCAOrigenEgreso
                    && q.StockKg > 0
                    && q.Estado == (int)EstadoTransmisionAVisec.Finalizado);

                var stockAcumulado = 0;
                Dictionary<string, int> stocksModificados = new Dictionary<string, int>();

                var stockTotalDisponible = stocks.Sum(s => s.StockKg);

                if (stockTotalDisponible < comando.StockSolicitado)
                {
                    resultado.Errores.Add("Stock", "No hay suficiente stock disponible para cubrir la cantidad solicitada.");
                    return resultado;
                }

                foreach (var stock in stocks.OrderBy(q => q.Id))
                {
                    if (stockAcumulado >= comando.StockSolicitado)
                        break;

                    var cantidadATomar = Math.Min(stock.StockKg.Value, comando.StockSolicitado - stockAcumulado);

                    stockAcumulado += cantidadATomar;

                    stock.StockKg = stock.StockKg - cantidadATomar;
                    stocksModificados.Add(stock.NumeroCTG, cantidadATomar);
                }
                resultado.Data = stocksModificados;
                Repositorio.GuardarCambios();
                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar DistribuirVisecStock.");
                resultado.Errores.Add("Error", ex.Message);
                return resultado;
            }
        }
    }
}