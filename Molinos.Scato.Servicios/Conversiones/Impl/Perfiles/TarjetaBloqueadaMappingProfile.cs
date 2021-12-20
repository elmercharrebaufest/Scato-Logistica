using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TarjetaBloqueadaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TarjetaBloqueadaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TarjetaBloqueada, TarjetaBloqueadaDto>()
                .ForMember(x => x.CentroId, mat => mat.MapFrom(a => a.Centro.Id));
            Mapper.CreateMap<TarjetaBloqueadaDto, TarjetaBloqueada>();
        }
    }
}
