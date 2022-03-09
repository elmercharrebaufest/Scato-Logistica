using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ConfiguracionGeneralMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ConfiguracionGeneralMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ConfiguracionGeneral, ConfiguracionGeneralDto>();
            Mapper.CreateMap<ConfiguracionGeneralDto, ConfiguracionGeneral>();
        }
    }
}
