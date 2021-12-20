using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMovimientoDeBines: ProcesadorCrear<CrearMovimientoDeBines, MovimientoDeBines>
    {
        public ProcesadorCrearMovimientoDeBines(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override MovimientoDeBines CrearEntidad(CrearMovimientoDeBines comando)
        {
            var ajuste = Conversor.Convertir<MovimientoDeBinesDto, MovimientoDeBines>(comando.Dto);
            if (comando.Dto.TipoAjuste.HasValue && comando.Dto.TipoAjuste.Value == TipoStockBines.Centro)
            {
                ajuste.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            }
            if (comando.Dto.TipoAjuste.HasValue && comando.Dto.TipoAjuste.Value == TipoStockBines.Productor)
            {
                ajuste.Proveedor = Repositorio.Obtener<Proveedor>(comando.Dto.ProveedorId);
                ajuste.CentroOrigen = Repositorio.Obtener<Centro>(comando.Dto.CentroOrigenId);
                comando.Dto.CentroId = comando.Dto.CentroOrigenId;
            }
            if (comando.Dto.TipoAjuste.HasValue && comando.Dto.TipoAjuste.Value == TipoStockBines.VinedoPropio)
            {
                ajuste.VinedoPropio = Repositorio.Obtener<VinedoPropio>(comando.Dto.VinedoPropioId);
                ajuste.CentroOrigen = Repositorio.Obtener<Centro>(comando.Dto.CentroOrigenId);
                comando.Dto.CentroId = comando.Dto.CentroOrigenId;
            }
            ajuste.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            ajuste.Numero = string.Format("{0}-{1}", Repositorio.ObtenerProyeccion<Centro, string>(x => x.Id == comando.Dto.CentroId, x => x.CodigoSAP), Repositorio.ObtenerNumeroDocumentoGenerado().ToString("D8"));

            return ajuste;
        }

        protected override void Validar(CrearMovimientoDeBines comando, Resultado resultado)
        {
        }
    }
}
