using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class VariedadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VariedadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Variedad, VariedadDto>();
            Mapper.CreateMap<VariedadDto, Variedad>();
        }
    }
}