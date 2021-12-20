using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ReciboMunicipalMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ReciboMunicipalMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ReciboMunicipal, ReciboMunicipalDto>();
            Mapper.CreateMap<ReciboMunicipalDto, ReciboMunicipal>();
        }
    }
}