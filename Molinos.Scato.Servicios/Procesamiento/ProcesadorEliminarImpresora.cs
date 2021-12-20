using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarImpresora : ProcesadorEliminar<EliminarImpresora, Impresora>
    {
        public ProcesadorEliminarImpresora(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarImpresora comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarImpresora comando, Resultado resultado)
        {
        }
    }
}
