using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    class TaraRomaneoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TaraRomaneoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TaraRomaneo, TaraRomaneoDto>()
                .ForMember(x => x.CentroId, mat => mat.MapFrom(taraRomaneo => taraRomaneo.Centro.Id));
            Mapper.CreateMap<TaraRomaneoDto, TaraRomaneo>();
        }
    }
}
