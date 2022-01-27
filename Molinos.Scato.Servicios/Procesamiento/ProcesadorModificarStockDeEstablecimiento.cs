using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarStockDeEstablecimiento : ProcesadorModificar<ModificarStockDeEstablecimiento>
    {
        public ProcesadorModificarStockDeEstablecimiento(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarStockDeEstablecimiento comando)
        {
            var stockDeEstablecimiento = Repositorio.Obtener<StockDeEstablecimiento>(comando.Dto.Id);
            stockDeEstablecimiento.FechaHasta = comando.Dto.FechaHasta;
            stockDeEstablecimiento.StockDeclarado = comando.Dto.StockDeclarado;
        }

        protected override void Validar(ModificarStockDeEstablecimiento comando, Resultado resultado)
        {
            if (Repositorio.Existe<StockDeEstablecimiento>(x => x.Id != comando.Dto.Id && x.CodigoEstablecimiento == comando.Dto.CodigoEstablecimiento && x.Cosecha == comando.Dto.Cosecha))
            {
                resultado.Error("CodigoEstablecimiento", string.Format(Textos.Error_Existente, Textos.Establecimiento_Codigo));
            }
            var fechaHastaEntidad = Repositorio.ObtenerProyeccion<StockDeEstablecimiento, DateTime>(x => x.Id == comando.Dto.Id,x => x.FechaHasta);
            if (fechaHastaEntidad > comando.Dto.FechaHasta)
            {
                resultado.Error("FechaHasta", string.Format(Textos.Error_FechaMayorIgualA, fechaHastaEntidad.ToString("dd/MM/yyyy")));
            }
            var stockUtilizado = Repositorio.Sumar<RegistroStockEPA>(x => x.PesoNeto, x => x.CodigoEstablecimiento == comando.Dto.CodigoEstablecimiento && x.Cosecha == comando.Dto.Cosecha);
                stockUtilizado += Repositorio.Sumar<RegistroStockOtrosPuertos>(x => x.PesoNeto, x => x.CodigoEstablecimiento == comando.Dto.CodigoEstablecimiento && x.Cosecha == comando.Dto.Cosecha);
            if (comando.Dto.StockDeclarado < stockUtilizado)
            {
                resultado.Error("StockDeclarado", Textos.Error_StockDeclaradoMenorAUtilizado);
            }
        }
    }
}
