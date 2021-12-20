using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarActividadPorDispositivo : ProcesadorModificar<ModificarActividadPorDispositivo>
    {
        public ProcesadorModificarActividadPorDispositivo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarActividadPorDispositivo comando)
        {
            var actividadPorDispositivo = Repositorio.Obtener<ActividadPorDispositivo>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, actividadPorDispositivo);
            actividadPorDispositivo.Workflow = Repositorio.Obtener<Workflow>(comando.Dto.WorkflowId);
            actividadPorDispositivo.PuestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.PuestoDeTrabajoId);
            actividadPorDispositivo.Actividad = actividadPorDispositivo.Actividad.Replace("V2", "");
            var videocamarasAEliminar = Repositorio.Listar<VideoCamara>(x => x.PuestoDeTrabajo == null && x.ActividadPorDispositivo == null);
            Repositorio.RemoverTodos(videocamarasAEliminar);
        }

        protected override void Validar(ModificarActividadPorDispositivo comando, Resultado resultado)
        {
            if (comando.Dto.WorkflowId == 0)
            {
                resultado.Error("WorkflowId", String.Format(Textos.Error_Requerido, Textos.Workflow));
            }
            if (Repositorio.Existe<ActividadPorDispositivo>(x => x.Id != comando.Dto.Id && x.Actividad == comando.Dto.Actividad && x.Workflow.Id == comando.Dto.WorkflowId && x.PuestoDeTrabajo.Id == comando.Dto.PuestoDeTrabajoId))
            {
                resultado.Error("", Textos.ActividadPorDispositivo_Existente);
            }
        }
    }
}
