using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CampoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CampoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Campo, CampoDto>();
            Mapper.CreateMap<CampoDto, Campo>();
        }
    }
}