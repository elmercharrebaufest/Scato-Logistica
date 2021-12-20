using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CabezalMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CabezalMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Cabezal, CabezalDto>();
            Mapper.CreateMap<CabezalDto, Cabezal>();
        }
    }
}