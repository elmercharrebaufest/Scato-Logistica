using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ControlDeTiempoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ControlDeTiempoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ControlDeTiempo, ControlDeTiempoDto>()
                .ForMember(t => t.WorkflowId, f => f.MapFrom(r => r.Workflow.Id))
                .ForMember(t => t.WorkflowCodigo, f => f.MapFrom(r => r.Workflow.Codigo))
                .ForMember(t => t.WorkflowDescripcion, f => f.MapFrom(r => r.Workflow.Descripcion));
            Mapper.CreateMap<ControlDeTiempoDto, ControlDeTiempo>();
        }
    }
}