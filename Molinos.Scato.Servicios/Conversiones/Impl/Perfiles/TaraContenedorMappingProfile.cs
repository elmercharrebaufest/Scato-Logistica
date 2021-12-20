using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TaraContenedorMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TaraContenedorMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TaraContenedor, TaraContenedorDto>();
            Mapper.CreateMap<TaraContenedorDto, TaraContenedor>();
        }
    }
}