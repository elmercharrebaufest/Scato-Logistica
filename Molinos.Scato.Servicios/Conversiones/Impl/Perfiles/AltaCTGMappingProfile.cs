using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AltaCTGMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AltaCTGMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AltaCTG, AltaCTGDto>();
            Mapper.CreateMap<AltaCTGDto, AltaCTG>();
        }
    }
}