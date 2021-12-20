using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class SubZonaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "SubZonaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<SubZona, SubZonaDto>();
            Mapper.CreateMap<SubZonaDto, SubZona>();
        }
    }
}
