using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearPermiso : ProcesadorCrear<CrearPermiso, Permiso>
    {
        public ProcesadorCrearPermiso(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Permiso CrearEntidad(CrearPermiso comando)
        {
            var permiso = Conversor.Convertir<PermisoDto, Permiso>(comando.Dto);
            return permiso;
        }

        protected override void Validar(CrearPermiso comando, Resultado resultado)
        {
            if (Repositorio.Existe<Permiso>(e => e.Descripcion == comando.Dto.Descripcion && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", Textos.Permiso_DescripcionExistente);
            }
        }
    }
}
