using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarVariedadPorVinedo : ProcesadorEliminar<EliminarVariedadPorVinedo, VariedadPorVinedo>
    {
        public ProcesadorEliminarVariedadPorVinedo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarVariedadPorVinedo comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarVariedadPorVinedo comando, Resultado resultado)
        {
        }
    }
}