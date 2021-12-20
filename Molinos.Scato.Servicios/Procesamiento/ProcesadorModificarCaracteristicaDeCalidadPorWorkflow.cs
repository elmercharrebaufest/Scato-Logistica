using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCaracteristicaDeCalidadPorWorkflow : ProcesadorCrear<ModificarCaracteristicaDeCalidadPorWorkflow, CaracteristicaDeCalidadPorWorkflow>
    {
        public ProcesadorModificarCaracteristicaDeCalidadPorWorkflow(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override CaracteristicaDeCalidadPorWorkflow CrearEntidad(ModificarCaracteristicaDeCalidadPorWorkflow comando)
        {
            return new CaracteristicaDeCalidadPorWorkflow
            {
                CaracteristicaDeCalidad = Repositorio.Obtener<CaracteristicaDeCalidad>(x => x.Id == comando.Dto.CaracteristicaDeCalidadId),
                Workflow = Repositorio.Obtener<Workflow>(x => x.Id == comando.Dto.WorkflowId)
            };
        }

        protected override void Validar(ModificarCaracteristicaDeCalidadPorWorkflow comando, Resultado resultado)
        {
            if (comando.Dto.CaracteristicaDeCalidadId != 0 && !Repositorio.Existe<CaracteristicaDeCalidad>(x => x.Id == comando.Dto.CaracteristicaDeCalidadId))
            {
                resultado.Error("CaracteristicaDeCalidadId", Textos.Error_Invalido);
            }
            if (comando.Dto.WorkflowId != 0 && !Repositorio.Existe<Workflow>(x => x.Id == comando.Dto.WorkflowId))
            {
                resultado.Error("WorkflowId", Textos.Error_Invalido);
            }
        }
    }
}
