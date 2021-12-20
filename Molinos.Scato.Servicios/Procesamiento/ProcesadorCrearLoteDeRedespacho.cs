using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLoteDeRedespacho : ProcesadorCrear<CrearLoteDeRedespacho, LoteDeRedespacho>
    {
        public ProcesadorCrearLoteDeRedespacho(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override LoteDeRedespacho CrearEntidad(CrearLoteDeRedespacho comando)
        {
            return new LoteDeRedespacho
            {
                Almacen = comando.Dto.Almacen,
                Centro = comando.Dto.Centro,
                Lote = comando.Dto.Lote,
                Material = comando.Dto.Material,
                Recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.Dto.InstanciaWorkflow),
                Stock = comando.Dto.Stock
            };
        }

        protected override void Validar(CrearLoteDeRedespacho comando, Resultado resultado)
        {
        }
    }
}