using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearRol : ProcesadorCrear<CrearRol, Rol>
    {
        public ProcesadorCrearRol(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Rol CrearEntidad(CrearRol comando)
        {
            var rolEditado = Conversor.Convertir<RolDto, Rol>(comando.Dto);

            IList<Permiso> permisosActuales = comando.Dto.PermisosAsociados.Select(permisoDto => Repositorio.Obtener<Permiso>(permisoDto.Id)).ToList();
            rolEditado.PermisosAsociados = permisosActuales;

            return rolEditado;
        }

        protected override void Validar(CrearRol comando, Resultado resultado)
        {
            if (Repositorio.Existe<Rol>(e => e.Descripcion == comando.Dto.Descripcion && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", Textos.Rol_DescripcionExistente);
            }
        }
    }
}
