using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAjusteStockBines: ProcesadorCrear<CrearAjusteStockBines, AjusteStockBines>
    {
        public ProcesadorCrearAjusteStockBines(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override AjusteStockBines CrearEntidad(CrearAjusteStockBines comando)
        {
            var ajuste = Conversor.Convertir<AjusteStockBinesDto, AjusteStockBines>(comando.Dto);
            ajuste.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            ajuste.Proveedor = Repositorio.Obtener<Proveedor>(comando.Dto.ProveedorId);
            ajuste.VinedoPropio = Repositorio.Obtener<VinedoPropio>(comando.Dto.VinedoPropioId);
            ajuste.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            
            return ajuste;
        }

        protected override void Validar(CrearAjusteStockBines comando, Resultado resultado)
        {
        }
    }
}
