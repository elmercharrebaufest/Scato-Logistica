using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearActividadPorDispositivo : ProcesadorCrear<CrearActividadPorDispositivo, ActividadPorDispositivo>
    {
        public ProcesadorCrearActividadPorDispositivo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ActividadPorDispositivo CrearEntidad(CrearActividadPorDispositivo comando)
        {
            var puestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(x => x.Id == comando.Dto.PuestoDeTrabajoId);
            var workflow = Repositorio.Obtener<Workflow>(x => x.Id == comando.Dto.WorkflowId);
            var dispositivoEditado = Conversor.Convertir<ActividadPorDispositivoDto, ActividadPorDispositivo>(comando.Dto);
            dispositivoEditado.PuestoDeTrabajo = puestoDeTrabajo;
            dispositivoEditado.Workflow = workflow;
            dispositivoEditado.Actividad = dispositivoEditado.Actividad.Replace("V2", "");
            var videocamarasAEliminar = Repositorio.Listar<VideoCamara>(x => x.PuestoDeTrabajo == null && x.ActividadPorDispositivo == null);
            Repositorio.RemoverTodos(videocamarasAEliminar);

            return dispositivoEditado;
        }

        protected override void Validar(CrearActividadPorDispositivo comando, Resultado resultado)
        {
            if (comando.Dto.WorkflowId == 0)
            {
                resultado.Error("WorkflowId", String.Format(Textos.Error_Requerido, Textos.Workflow));
            }
            if (Repositorio.Existe<ActividadPorDispositivo>(x => x.Actividad == comando.Dto.Actividad && x.Workflow.Id == comando.Dto.WorkflowId && x.PuestoDeTrabajo.Id == comando.Dto.PuestoDeTrabajoId))
            {
                resultado.Error(string.Empty, Textos.ActividadPorDispositivo_Existente);
            }
        }
    }
}
