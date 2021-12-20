using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarUsuarioUltimoLogin : ProcesadorModificar<ModificarUsuarioUltimoLogin>
    {
        public ProcesadorModificarUsuarioUltimoLogin(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarUsuarioUltimoLogin comando)
        {
            var usuarioEditado = Repositorio.Obtener<Usuario>(x => x.NombreUsuario == comando.Usuario);
            if(!(usuarioEditado == null)){ 
            usuarioEditado.UltimoLogin = DateTime.Now;
            }
        }

        protected override void Validar(ModificarUsuarioUltimoLogin comando, Resultado resultado)
        {
        }
    }
}
