using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AgenteControlPrivadoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AgenteControlPrivadoProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AgenteControlPrivado, AgenteControlPrivadoDto>();
            Mapper.CreateMap<AgenteControlPrivadoDto, AgenteControlPrivado>();
        }
    }
}
