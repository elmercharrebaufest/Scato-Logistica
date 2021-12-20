using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ProvinciaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ProvinciaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Provincia, ProvinciaDto>();
            Mapper.CreateMap<ProvinciaDto, Provincia>();
        }
    }
}
