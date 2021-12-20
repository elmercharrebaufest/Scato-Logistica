using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ActividadConCargaAutomaticaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ActividadConCargaAutomaticaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ActividadConCargaAutomatica, ActividadConCargaAutomaticaDto>()
                  .ForMember(t => t.WorkflowId, f => f.MapFrom(r => r.Workflow.Id))
                  .ForMember(t => t.WorkflowCodigo, f => f.MapFrom(r => r.Workflow.Codigo))
                  .ForMember(t => t.WorkflowDescripcion, f => f.MapFrom(r => r.Workflow.Descripcion));
            Mapper.CreateMap<ActividadConCargaAutomaticaDto, ActividadConCargaAutomatica>();
        }
    }
}
