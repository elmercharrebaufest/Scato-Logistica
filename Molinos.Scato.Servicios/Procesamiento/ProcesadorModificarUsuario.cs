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
    public class ProcesadorModificarUsuario : ProcesadorModificar<ModificarUsuario>
    {
        public ProcesadorModificarUsuario(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarUsuario comando)
        {
            var usuarioEditado = Repositorio.Obtener<Usuario>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, usuarioEditado);

            usuarioEditado.CentrosAsociados.Clear();
            usuarioEditado.RolesAsociados.Clear();
            IList<Centro> centrosActuales = comando.Dto.CentrosAsociados.Select(centroDto => Repositorio.ObtenerUnchanged<Centro>(centroDto.Id)).ToList();

            IList<Rol> rolesActuales = comando.Dto.RolesAsociados.Select(rolDto => Repositorio.ObtenerUnchanged<Rol>(rolDto.Id)).ToList();

            usuarioEditado.RolesAsociados = rolesActuales;
            usuarioEditado.CentrosAsociados = centrosActuales;
        }

        protected override void Validar(ModificarUsuario comando, Resultado resultado)
        {
            if (Repositorio.Existe<Usuario>(e => e.NombreUsuario == comando.Dto.NombreUsuario && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("NombreUsuario", Textos.Usuario_NombreUsuarioExistente);
            }
        }
    }
}
