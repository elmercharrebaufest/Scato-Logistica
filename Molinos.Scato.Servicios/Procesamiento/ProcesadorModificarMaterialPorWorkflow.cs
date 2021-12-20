using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarMaterialPorWorkflow : ProcesadorModificar<ModificarMaterialPorWorkflow>
    {
        public ProcesadorModificarMaterialPorWorkflow(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarMaterialPorWorkflow comando)
        {
            var materialPorWorkflow = Repositorio.Obtener<MaterialPorWorkflow>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, materialPorWorkflow);
            if (materialPorWorkflow.Material.Id != comando.Dto.MaterialId)
            {
                materialPorWorkflow.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            }
            if (materialPorWorkflow.Centro.Id != comando.Dto.CentroId)
            {
                materialPorWorkflow.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            }
            if (materialPorWorkflow.Workflow.Id != comando.Dto.WorkflowId)
            {
                materialPorWorkflow.Workflow = Repositorio.Obtener<Workflow>(comando.Dto.WorkflowId);
            }
            if(materialPorWorkflow.Cliente == null || materialPorWorkflow.Cliente.Id != comando.Dto.ClienteId)
            {
                materialPorWorkflow.Cliente = Repositorio.Obtener<Cliente>(comando.Dto.ClienteId);
            }
            materialPorWorkflow.EnviaASapAlmacenPredeterminado = comando.Dto.EnviaASapAlmacenPredeterminado;
        }

        protected override void Validar(ModificarMaterialPorWorkflow comando, Resultado resultado)
        {
            if (comando.Dto.MaterialId != 0 && !Repositorio.Existe<Material>(x => x.Id == comando.Dto.MaterialId))
            {
                resultado.Error("MaterialId", Textos.Error_Invalido);
            }
            if (comando.Dto.WorkflowId != 0 && !Repositorio.Existe<Workflow>(x => x.Id == comando.Dto.WorkflowId))
            {
                resultado.Error("WorkflowId", Textos.Error_Invalido);
            }
            if (Repositorio.Existe<MaterialPorWorkflow>( e => e.Material.Id == comando.Dto.MaterialId && (e.Workflow.Id == comando.Dto.WorkflowId) && (e.Centro.Id == comando.Dto.CentroId) && ((e.Cliente == null && comando.Dto.ClienteId == 0) || (e.Cliente != null && e.Cliente.Id == comando.Dto.ClienteId)) && e.Id != comando.Dto.Id))
            {
                resultado.Error("MaterialId", string.Format(Textos.MaterialPorWorkflow_Existente, Textos.Descripcion));
            }
        }
    }
}
