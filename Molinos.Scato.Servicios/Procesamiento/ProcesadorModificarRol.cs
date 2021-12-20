using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRol : ProcesadorModificar<ModificarRol>
    {
        public ProcesadorModificarRol(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarRol comando)
        {
            var rolEditado = Repositorio.Obtener<Rol>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, rolEditado);

            rolEditado.PermisosAsociados.Clear();
            IList<Permiso> permisosActuales = comando.Dto.PermisosAsociados.Select(permisoDto => Repositorio.ObtenerUnchanged<Permiso>(permisoDto.Id)).ToList();

            rolEditado.PermisosAsociados = permisosActuales;
        }

        protected override void Validar(ModificarRol comando, Resultado resultado)
        {
            if (Repositorio.Existe<Rol>(e => e.Descripcion == comando.Dto.Descripcion && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", Textos.Rol_DescripcionExistente);
            }
        }
    }
}
