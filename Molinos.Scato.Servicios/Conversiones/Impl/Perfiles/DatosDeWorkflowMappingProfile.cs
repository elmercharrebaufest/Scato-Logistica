using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DatosDeWorkflowMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DatosDeWorkflowMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DatosDeWorkflow, DatosDeWorkflowDto>();
            Mapper.CreateMap<DatosDeWorkflowDto, DatosDeWorkflow>();
        }
    }
}
