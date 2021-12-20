using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class SugerenciaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "SugerenciaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Sugerencia, SugerenciaDto>();
            Mapper.CreateMap<SugerenciaDto, Sugerencia>();
        }
    }
}
