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
    public class ProcesadorModificarUsuarioMatricula : ProcesadorModificar<ModificarUsuarioMatricula>
    {
        public ProcesadorModificarUsuarioMatricula(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarUsuarioMatricula comando)
        {
            var usuario = Repositorio.Obtener<Usuario>(comando.Dto.Id);

            if (usuario != null)
            {
                usuario.Matricula = comando.Dto.Matricula;
                usuario.Firma = comando.Dto.Firma;
                usuario.FirmaImagen = comando.Dto.FirmaImagen;
            }
        }

        protected override void Validar(ModificarUsuarioMatricula comando, Resultado resultado)
        {

        }

    }
}
