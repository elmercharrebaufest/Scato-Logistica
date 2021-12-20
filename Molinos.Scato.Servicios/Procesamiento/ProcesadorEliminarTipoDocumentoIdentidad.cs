using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarTipoDocumentoIdentidad : ProcesadorEliminar<EliminarTipoDocumentoIdentidad, TipoDocumentoIdentidad>
    {
        public ProcesadorEliminarTipoDocumentoIdentidad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarTipoDocumentoIdentidad comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarTipoDocumentoIdentidad comando, Resultado resultado)
        {
        }
    }
}
