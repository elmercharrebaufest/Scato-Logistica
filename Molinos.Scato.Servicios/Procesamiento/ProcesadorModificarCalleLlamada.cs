using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCalleLlamada : ProcesadorModificar<ModificarCalleLlamada>
    {
        public ProcesadorModificarCalleLlamada(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCalleLlamada comando)
        {
            var calle = Repositorio.Obtener<Calle>(comando.Dto.Id);
            calle.FechaLLamada = comando.Dto.FechaLLamada;
            calle.Bloqueada = comando.Dto.Bloqueada;
        }

        protected override void Validar(ModificarCalleLlamada comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Calle>(x => x.Id == comando.Dto.Id))
            {
                resultado.Error(string.Empty, Textos.Calle_Inexistente);
            }
        }
    }
}