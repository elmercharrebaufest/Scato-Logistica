using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMaterialPorWorkflow : ProcesadorCrear<CrearMaterialPorWorkflow, MaterialPorWorkflow>
    {
        public ProcesadorCrearMaterialPorWorkflow(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override MaterialPorWorkflow CrearEntidad(CrearMaterialPorWorkflow comando)
        {
            return new MaterialPorWorkflow
            {
                Material = Repositorio.Obtener<Material>(x => x.Id == comando.Dto.MaterialId),
                Centro = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroId),
                Workflow = Repositorio.Obtener<Workflow>(x => x.Id == comando.Dto.WorkflowId),
                Cliente = Repositorio.Obtener<Cliente>(x => x.Id == comando.Dto.ClienteId),
                EnviaASapAlmacenPredeterminado = comando.Dto.EnviaASapAlmacenPredeterminado
            };
        }

        protected override void Validar(CrearMaterialPorWorkflow comando, Resultado resultado)
        {
            if (comando.Dto.MaterialId != 0 && !Repositorio.Existe<Material>(x => x.Id == comando.Dto.MaterialId))
            {
                resultado.Error("MaterialId", Textos.Error_Invalido);
            }
            if (comando.Dto.WorkflowId != 0 && !Repositorio.Existe<Workflow>(x => x.Id == comando.Dto.WorkflowId))
            {
                resultado.Error("WorkflowId", Textos.Error_Invalido);
            }
            if (
                Repositorio.Existe<MaterialPorWorkflow>(
                    e => e.Material.Id == comando.Dto.MaterialId && (e.Workflow.Id == comando.Dto.WorkflowId) &&
                    (e.Centro.Id == comando.Dto.CentroId) && ((e.Cliente == null && comando.Dto.ClienteId == 0) || (e.Cliente != null && e.Cliente.Id == comando.Dto.ClienteId))))
            {
                resultado.Error("MaterialId", string.Format(Textos.MaterialPorWorkflow_Existente, Textos.Descripcion));
            }

        }
    }
}
