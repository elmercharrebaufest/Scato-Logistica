using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarHuellaDigital : ProcesadorModificar<EliminarHuellaDigital>
    {
        public ProcesadorEliminarHuellaDigital(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(EliminarHuellaDigital comando)
        {
            var huellaDigital = Repositorio.Obtener<HuellaDigital>(comando.Id);

            huellaDigital.Estado = false;
        }

        protected override void Validar(EliminarHuellaDigital comando, Resultado resultado)
        {
        }
    }
}
