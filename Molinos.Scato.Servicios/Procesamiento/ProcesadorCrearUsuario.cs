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
    public class ProcesadorCrearUsuario : ProcesadorCrear<CrearUsuario, Usuario>
    {
        public ProcesadorCrearUsuario(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Usuario CrearEntidad(CrearUsuario comando)
        {
            var usarioEditado = Conversor.Convertir<UsuarioDto, Usuario>(comando.Dto);

            IList<Rol> rolesActuales = comando.Dto.RolesAsociados.Select(rolDto => Repositorio.Obtener<Rol>(rolDto.Id)).ToList();
            usarioEditado.RolesAsociados = rolesActuales;

            IList<Centro> centrosActuales = comando.Dto.CentrosAsociados.Select(centroDto => Repositorio.Obtener<Centro>(centroDto.Id)).ToList();
            usarioEditado.CentrosAsociados = centrosActuales;

            return usarioEditado;
        }

        protected override void Validar(CrearUsuario comando, Resultado resultado)
        {
            if (Repositorio.Existe<Usuario>(e => e.NombreUsuario == comando.Dto.NombreUsuario ))
            {
                resultado.Error("NombreUsuario", Textos.Usuario_NombreUsuarioExistente);
            }
        }
    }
}
