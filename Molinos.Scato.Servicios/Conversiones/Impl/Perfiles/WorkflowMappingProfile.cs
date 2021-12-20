using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class WorkflowMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "WorkflowMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Workflow, WorkflowDto>().ForMember(x => x.CentroId, c => c.MapFrom(w => w.Centro.Id));
            Mapper.CreateMap<WorkflowDto, Workflow>();
        }
    }
}