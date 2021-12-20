using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarMaterialPorWorkflow : ProcesadorEliminar<EliminarMaterialPorWorkflow, MaterialPorWorkflow>
    {
        public ProcesadorEliminarMaterialPorWorkflow(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarMaterialPorWorkflow comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarMaterialPorWorkflow comando, Resultado resultado)
        {
        }
    }
}