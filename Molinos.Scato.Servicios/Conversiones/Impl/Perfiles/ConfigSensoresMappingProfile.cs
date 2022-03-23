using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ConfigSensoresMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ConfigSensoresMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ConfigSensores, ConfigSensoresDto>();
            Mapper.CreateMap<ConfigSensoresDto, ConfigSensores>();
        }
    }
}
