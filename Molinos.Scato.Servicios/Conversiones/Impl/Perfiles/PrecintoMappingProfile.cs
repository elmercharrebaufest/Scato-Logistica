using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PrecintoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PrecintoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Precinto, PrecintoDto>();
            Mapper.CreateMap<PrecintoDto, Precinto>();
        }
    }
}
