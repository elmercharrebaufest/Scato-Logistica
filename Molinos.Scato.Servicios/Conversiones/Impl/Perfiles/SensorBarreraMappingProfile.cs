using AutoMapper;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class SensorBarreraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "SensorBarreraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<SensorBarrera, SensorBarreraDto>();
            Mapper.CreateMap<SensorBarreraDto, SensorBarrera>();
        }
    }
}
