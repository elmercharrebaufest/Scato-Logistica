using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ActividadPorDispositivoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ActividadPorDispositivoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ActividadPorDispositivo, ActividadPorDispositivoDto>()
                  .ForMember(t => t.PuestoDeTrabajoId, f => f.MapFrom(r => r.PuestoDeTrabajo.Id))
                  .ForMember(t => t.PuestoDeTrabajoDescripcion, f => f.MapFrom(r => r.PuestoDeTrabajo.NombrePuesto))
                  .ForMember(t => t.CentroId, f => f.MapFrom(r => r.PuestoDeTrabajo.Centro.Id))
                  .ForMember(t => t.WorkflowId, f => f.MapFrom(r => r.Workflow.Id))
                  .ForMember(t => t.WorkflowCodigo, f => f.MapFrom(r => r.Workflow.Codigo))
                  .ForMember(t => t.WorkflowDescripcion, f => f.MapFrom(r => r.Workflow.Descripcion));
            Mapper.CreateMap<ActividadPorDispositivoDto, ActividadPorDispositivo>();
        }
    }
}
