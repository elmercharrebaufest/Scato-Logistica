using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCaracteristicaDeCalidadPorWorkflow : ProcesadorCrear<CrearCaracteristicaDeCalidadPorWorkflow, CaracteristicaDeCalidadPorWorkflow>
    {
        public ProcesadorCrearCaracteristicaDeCalidadPorWorkflow(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override CaracteristicaDeCalidadPorWorkflow CrearEntidad(CrearCaracteristicaDeCalidadPorWorkflow comando)
        {
            return new CaracteristicaDeCalidadPorWorkflow
            {
                CaracteristicaDeCalidad = Repositorio.Obtener<CaracteristicaDeCalidad>(x => x.Id == comando.Dto.CaracteristicaDeCalidadId),
                Workflow = Repositorio.Obtener<Workflow>(x => x.Id == comando.Dto.WorkflowId)
            };
        }

        protected override void Validar(CrearCaracteristicaDeCalidadPorWorkflow comando, Resultado resultado)
        {
            if (comando.Dto.CaracteristicaDeCalidadId != 0 && !Repositorio.Existe<CaracteristicaDeCalidad>(x => x.Id == comando.Dto.CaracteristicaDeCalidadId))
            {
                resultado.Error("CaracteristicaDeCalidadId", Textos.Error_Invalido);
            }
            if (comando.Dto.WorkflowId != 0 && !Repositorio.Existe<Workflow>(x => x.Id == comando.Dto.WorkflowId))
            {
                resultado.Error("WorkflowId", Textos.Error_Invalido);
            }
            if (
                Repositorio.Existe<CaracteristicaDeCalidadPorWorkflow>(
                    e => e.CaracteristicaDeCalidad.MaterialPorCentro.Material.Id == comando.Dto.MaterialId && e.Workflow.Id == comando.Dto.WorkflowId))
            {
                resultado.Error("CaracteristicaDeCalidadId", string.Format(Textos.CaracteristicaDeCalidadPorWorkflow_Existente, Textos.Descripcion));
            }
            if (
                !Repositorio.Existe<MaterialPorWorkflow>(
                    e => e.Material.Id == comando.Dto.MaterialId && e.Workflow.Id == comando.Dto.WorkflowId))
            {
                resultado.Error("MaterialId", string.Format(Textos.MaterialPorWorkflow_NoExistente, Textos.Descripcion));
            }
        }
    }
}
