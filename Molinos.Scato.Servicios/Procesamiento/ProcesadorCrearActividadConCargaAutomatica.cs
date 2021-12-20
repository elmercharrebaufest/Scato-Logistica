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
    public class ProcesadorCrearActividadConCargaAutomatica : ProcesadorCrear<CrearActividadConCargaAutomatica, ActividadConCargaAutomatica>
    {
        public ProcesadorCrearActividadConCargaAutomatica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ActividadConCargaAutomatica CrearEntidad(CrearActividadConCargaAutomatica comando)
        {
            var workflow = Repositorio.Obtener<Workflow>(x => x.Id == comando.Dto.WorkflowId);
            var dispositivoEditado = Conversor.Convertir<ActividadConCargaAutomaticaDto, ActividadConCargaAutomatica>(comando.Dto);
            dispositivoEditado.Workflow = workflow;
            return dispositivoEditado;
        }

        protected override void Validar(CrearActividadConCargaAutomatica comando, Resultado resultado)
        {
            if (comando.Dto.WorkflowId == 0)
            {
                resultado.Error("WorkflowId", String.Format(Textos.Error_Requerido, Textos.Workflow));
            }
            if (Repositorio.Existe<ActividadConCargaAutomatica>(x => x.Actividad == comando.Dto.Actividad && x.Workflow.Id == comando.Dto.WorkflowId && x.Workflow.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error(string.Empty, Textos.ActividadConCargaAutomatica_Existente);
            }
        }
    }
}
