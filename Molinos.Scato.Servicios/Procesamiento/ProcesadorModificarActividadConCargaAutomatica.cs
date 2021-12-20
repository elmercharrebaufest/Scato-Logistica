using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarActividadConCargaAutomatica : ProcesadorModificar<ModificarActividadConCargaAutomatica>
    {
        public ProcesadorModificarActividadConCargaAutomatica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarActividadConCargaAutomatica comando)
        {
            var actividadConCargaAutomatica = Repositorio.Obtener<ActividadConCargaAutomatica>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, actividadConCargaAutomatica);
            actividadConCargaAutomatica.Workflow = Repositorio.Obtener<Workflow>(comando.Dto.WorkflowId);
        }

        protected override void Validar(ModificarActividadConCargaAutomatica comando, Resultado resultado)
        {
            if (comando.Dto.WorkflowId == 0)
            {
                resultado.Error("WorkflowId", String.Format(Textos.Error_Requerido, Textos.Workflow));
            }
            if (Repositorio.Existe<ActividadConCargaAutomatica>(x => x.Id != comando.Dto.Id && x.Actividad == comando.Dto.Actividad && x.Workflow.Id == comando.Dto.WorkflowId && x.Workflow.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("", Textos.ActividadConCargaAutomatica_Existente);
            }
        }
    }
}
