using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarNirs : ProcesadorEliminar<EliminarNirs, Nirs>
    {
        public ProcesadorEliminarNirs(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarNirs comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarNirs comando, Resultado resultado)
        {
        }
    }
}
