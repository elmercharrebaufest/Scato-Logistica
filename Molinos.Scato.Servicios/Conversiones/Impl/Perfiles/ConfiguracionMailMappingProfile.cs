using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ConfiguracionMailMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ConfiguracionMailMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ConfiguracionMail, ConfiguracionMailDto>();
            Mapper.CreateMap<ConfiguracionMailDto, ConfiguracionMail>();
        }
    }
}
