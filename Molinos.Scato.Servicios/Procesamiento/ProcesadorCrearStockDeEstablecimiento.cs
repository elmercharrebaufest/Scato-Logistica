using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearStockDeEstablecimiento : ProcesadorCrear<CrearStockDeEstablecimiento, StockDeEstablecimiento>
    {
        public ProcesadorCrearStockDeEstablecimiento(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override StockDeEstablecimiento CrearEntidad(CrearStockDeEstablecimiento comando)
        {
            var entidad = new StockDeEstablecimiento
                {
                    CodigoEstablecimiento = comando.Dto.CodigoEstablecimiento,
                    Cosecha = comando.Dto.Cosecha,
                    FechaDesde = comando.Dto.FechaDesde,
                    FechaHasta = comando.Dto.FechaHasta,
                    StockDeclarado = comando.Dto.StockDeclarado
                };
            return entidad;
        }

        protected override void Validar(CrearStockDeEstablecimiento comando, Resultado resultado)
        {
            if (Repositorio.Existe<StockDeEstablecimiento>(x => x.CodigoEstablecimiento == comando.Dto.CodigoEstablecimiento && x.Cosecha == comando.Dto.Cosecha))
            {
                resultado.Error("", string.Format(Textos.Error_ExistenteStockEstablecimiento,Textos.Establecimiento_Codigo,Textos.Campaña));
            }
            if (!Repositorio.Existe<Establecimiento>(x => x.CodigoDeEstablecimiento == comando.Dto.CodigoEstablecimiento))
            {
                resultado.Error("CodigoEstablecimiento", Textos.Error_NoExisteEstablecimiento);
            }
        }
    }
}