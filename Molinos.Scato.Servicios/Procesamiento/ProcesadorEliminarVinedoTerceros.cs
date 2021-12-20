using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarVinedoTerceros : ProcesadorEliminar<EliminarVinedoTerceros, VinedoTerceros>
    {
        public ProcesadorEliminarVinedoTerceros(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
            //TODO
        }

        protected override int IdEntidad(EliminarVinedoTerceros comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarVinedoTerceros comando, Resultado resultado)
        {
            //TODO
        }
    }
}
