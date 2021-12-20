using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ZonaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ZonaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Zona, ZonaDto>();
            Mapper.CreateMap<ZonaDto, Zona>();
        }
    }
}
