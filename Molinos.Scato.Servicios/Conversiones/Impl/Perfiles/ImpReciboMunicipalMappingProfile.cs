using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpReciboMunicipalMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpReciboMunicipalMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpReciboMunicipal, ImpReciboMunicipalDto>();
            Mapper.CreateMap<ImpReciboMunicipalDto, ImpReciboMunicipal>();
        }
    }
}
