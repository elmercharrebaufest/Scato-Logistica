using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CupoMobileMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CupoMobileMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CupoMobile, CupoMobileDto>();
            Mapper.CreateMap<CupoMobileDto, CupoMobile>();
        }
    }
}
