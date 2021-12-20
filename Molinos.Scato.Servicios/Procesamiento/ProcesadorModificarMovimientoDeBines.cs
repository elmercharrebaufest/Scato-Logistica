using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarMovimientoDeBines:ProcesadorModificar<ModificarMovimientoDeBines>
    {
        public ProcesadorModificarMovimientoDeBines(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarMovimientoDeBines comando)
        {
            var ajuste = Repositorio.Obtener<MovimientoDeBines>(comando.Dto.Id);
            if (comando.Dto.TipoAjuste.HasValue && comando.Dto.TipoAjuste.Value == TipoStockBines.Productor && (ajuste.Proveedor == null || ajuste.Proveedor.Id != comando.Dto.ProveedorId))
            {
                ajuste.Proveedor = Repositorio.Obtener<Proveedor>(comando.Dto.ProveedorId);
                ajuste.Centro = null;
                ajuste.VinedoPropio = null;
            }
            if (comando.Dto.TipoAjuste.HasValue && comando.Dto.TipoAjuste.Value == TipoStockBines.Centro && (ajuste.Centro == null || ajuste.Centro.Id != comando.Dto.CentroId))
            {
                ajuste.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
                ajuste.Proveedor = null;
                ajuste.VinedoPropio = null;
            }
            if (comando.Dto.TipoAjuste.HasValue && comando.Dto.TipoAjuste.Value == TipoStockBines.VinedoPropio && (ajuste.VinedoPropio == null || ajuste.VinedoPropio.Id != comando.Dto.VinedoPropioId))
            {
                ajuste.VinedoPropio = Repositorio.Obtener<VinedoPropio>(comando.Dto.VinedoPropioId);
                ajuste.Proveedor = null;
                ajuste.Centro = null;
            }
            if (ajuste.Material == null || ajuste.Material.Id != comando.Dto.MaterialId)
            {
                ajuste.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            }
            ajuste.Fecha = comando.Dto.Fecha;
            ajuste.TipoAjuste = comando.Dto.TipoAjuste.Value;
            ajuste.Fecha = comando.Dto.Fecha;
            ajuste.Cantidad = comando.Dto.Cantidad;
            ajuste.Observaciones = comando.Dto.Observaciones;
            ajuste.Movimiento = comando.Dto.Movimiento.Value;
        }

        protected override void Validar(ModificarMovimientoDeBines comando, Resultado resultado)
        {
            if (comando.Dto.MaterialId != 0 && !Repositorio.Existe<Material>(x => x.Id != comando.Dto.MaterialId))
            {
                resultado.Error("MaterialId", Textos.Error_Invalido);
            }
        }
    }
}
