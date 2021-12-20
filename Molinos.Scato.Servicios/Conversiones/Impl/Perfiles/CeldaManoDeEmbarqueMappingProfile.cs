using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CeldaManoDeEmbarqueMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CeldaManoDeEmbarqueMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CeldaManoDeEmbarque, CeldaManoDeEmbarqueDto>();
            Mapper.CreateMap<CeldaManoDeEmbarqueDto, CeldaManoDeEmbarque>();
        }
    }
}
