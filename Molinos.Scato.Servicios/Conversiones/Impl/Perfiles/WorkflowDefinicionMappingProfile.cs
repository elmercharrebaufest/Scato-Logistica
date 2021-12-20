using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class WorkflowDefinicionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "WorkflowDefinicionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<WorkflowDefinicion, WorkflowDefinicionDto>();
            Mapper.CreateMap<WorkflowDefinicionDto, WorkflowDefinicion>();
        }
    }

}