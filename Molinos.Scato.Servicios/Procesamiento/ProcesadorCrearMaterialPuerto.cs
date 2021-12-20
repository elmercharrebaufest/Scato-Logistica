using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMaterialPuerto : ProcesadorCrear<CrearMaterialPuerto, MaterialPuerto>
    {
        public ProcesadorCrearMaterialPuerto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override MaterialPuerto CrearEntidad(CrearMaterialPuerto comando)
        {
            var entidad = Conversor.Convertir<MaterialPuertoDto, MaterialPuerto>(comando.Dto);
            if(comando.Dto.Almacen_Id != null && comando.Dto.Almacen_Id != 0)
            {
                entidad.Almacen = Repositorio.Obtener<Almacen>(comando.Dto.Almacen_Id);
            }
            return entidad;
        }

        protected override void Validar(CrearMaterialPuerto comando, Resultado resultado)
        {
            if (Repositorio.Existe<MaterialPuerto>(e => e.Descripcion == comando.Dto.Descripcion))
            {
                resultado.Error("Descripcion", Textos.Error_Existente);
            }
        }
    }
}
