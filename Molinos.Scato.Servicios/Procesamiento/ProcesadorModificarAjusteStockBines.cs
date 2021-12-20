using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarAjusteStockBines:ProcesadorModificar<ModificarAjusteStockBines>
    {
        public ProcesadorModificarAjusteStockBines(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarAjusteStockBines comando)
        {
            var ajuste = Repositorio.Obtener<AjusteStockBines>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, ajuste);
            if (ajuste.Proveedor == null || ajuste.Proveedor.Id != comando.Dto.ProveedorId)
            {
                ajuste.Proveedor = Repositorio.Obtener<Proveedor>(comando.Dto.ProveedorId);
            }
            if (ajuste.Observaciones == null || ajuste.Observaciones != comando.Dto.Observaciones)
            {
                ajuste.Observaciones = comando.Dto.Observaciones;
            }
            if (ajuste.Centro == null || ajuste.Centro.Id != comando.Dto.CentroId)
            {
                ajuste.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            }
            if (ajuste.VinedoPropio == null || ajuste.VinedoPropio.Id != comando.Dto.VinedoPropioId)
            {
                ajuste.VinedoPropio = Repositorio.Obtener<VinedoPropio>(comando.Dto.VinedoPropioId);
            }
            if (ajuste.Material == null || ajuste.Material.Id != comando.Dto.MaterialId)
            {
                ajuste.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            }
            if (ajuste.Stock == 0)
            {
                ajuste.Stock = comando.Dto.Stock;
            }
            ajuste.Fecha = comando.Dto.Fecha;
            ajuste.TipoAjuste = comando.Dto.TipoAjuste;


        }

        protected override void Validar(ModificarAjusteStockBines comando, Resultado resultado)
        {
            if (comando.Dto.MaterialId != 0 && !Repositorio.Existe<Material>(x => x.Id != comando.Dto.MaterialId))
            {
                resultado.Error("MaterialId", Textos.Error_Invalido);
            }
        }
    }
}
