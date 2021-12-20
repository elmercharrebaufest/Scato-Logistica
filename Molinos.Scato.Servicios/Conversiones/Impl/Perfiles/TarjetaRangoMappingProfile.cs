using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TarjetaRangoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TarjetaRangoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TarjetaRango, TarjetaRangoDto>()
                .ForMember(x => x.CentroId, mat => mat.MapFrom(a => a.Centro.Id));
            Mapper.CreateMap<TarjetaRangoDto, TarjetaRango>();
        }
    }
}
