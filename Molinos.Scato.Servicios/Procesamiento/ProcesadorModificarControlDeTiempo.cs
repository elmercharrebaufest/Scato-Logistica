using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarControlDeTiempo : ProcesadorModificar<ModificarControlDeTiempo>
    {
        public ProcesadorModificarControlDeTiempo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarControlDeTiempo comando)
        {
            var control = Repositorio.Obtener<ControlDeTiempo>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, control);
            control.Workflow = Repositorio.Obtener<Workflow>(comando.Dto.WorkflowId);
        }

        protected override void Validar(ModificarControlDeTiempo comando, Resultado resultado)
        {
            if (comando.Dto.WorkflowId == 0)
            {
                resultado.Error("WorkflowId", String.Format(Textos.Error_Requerido, Textos.Workflow));
            }
            else if (Repositorio.Existe<ControlDeTiempo>(x => comando.Dto.ActividadDesde == comando.Dto.ActividadHasta))
            {
                resultado.Error("", Textos.ControlDeTiempo_Iguales);
            }
            else if (Repositorio.Existe<ControlDeTiempo>(x => x.Id != comando.Dto.Id && x.ActividadDesde == comando.Dto.ActividadDesde && x.ActividadHasta == comando.Dto.ActividadHasta && x.Workflow.Id == comando.Dto.WorkflowId))
            {
                resultado.Error("", Textos.ControlDeTiempo_Existente);
            }
            else if (Repositorio.Existe<ControlDeTiempo>(x => x.Id != comando.Dto.Id && x.CodigoControl == comando.Dto.CodigoControl && x.Workflow.Id == comando.Dto.WorkflowId))
            {
                resultado.Error("", Textos.ControlDeTiempo_CodigoExistente);
            }
        }
    }
}
