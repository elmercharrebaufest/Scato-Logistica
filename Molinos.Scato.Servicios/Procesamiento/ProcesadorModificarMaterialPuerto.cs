using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarMaterialPuerto : ProcesadorModificar<ModificarMaterialPuerto>
    {
        public ProcesadorModificarMaterialPuerto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarMaterialPuerto comando)
        {
            var material = Repositorio.Obtener<MaterialPuerto>(comando.Dto.Id);
            if (comando.Dto.Almacen_Id != null && comando.Dto.Almacen_Id != 0)
            {
                material.Almacen = Repositorio.Obtener<Almacen>(comando.Dto.Almacen_Id);
            }
            else
            {
                material.Almacen = null;
            }
            Conversor.Convertir(comando.Dto, material);
        }

        protected override void Validar(ModificarMaterialPuerto comando, Resultado resultado)
        {

        }
    }
}
