using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarVapor : ProcesadorEliminar<EliminarVapor, Vapor>
    {
        public ProcesadorEliminarVapor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarVapor comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarVapor comando, Resultado resultado)
        {
        }
    }
}
