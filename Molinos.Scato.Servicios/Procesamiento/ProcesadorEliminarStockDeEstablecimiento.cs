using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarStockDeEstablecimiento : ProcesadorEliminar<EliminarStockDeEstablecimiento, StockDeEstablecimiento>
    {
        public ProcesadorEliminarStockDeEstablecimiento(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarStockDeEstablecimiento comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarStockDeEstablecimiento comando, Resultado resultado)
        {
            var stock = Repositorio.Obtener<StockDeEstablecimiento>(comando.Id);
            var stockUtilizado = Repositorio.Sumar<RegistroStockEPA>(x => x.PesoNeto, x => x.CodigoEstablecimiento == stock.CodigoEstablecimiento && x.Cosecha == stock.Cosecha);
            if (stockUtilizado > 0)
            {
                resultado.Error("", Textos.Error_EliminarStockEstablecimientoUsado);
            }
        }
    }
}