using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    class MicroMuestrasPorCasilleroMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MicroMuestrasPorCasilleroMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MicroMuestrasPorCasillero, MicroMuestrasPorCasilleroDto>()
                  .ForMember(x => x.MuestraId, mat => mat.MapFrom(microMuestra => microMuestra.Muestra.Id))
                  .ForMember(x => x.CasilleroId, mat => mat.MapFrom(microMuestra => microMuestra.Casillero.Id))
                  .ForMember(x => x.CasilleroNumero, mat => mat.MapFrom(microMuestra => microMuestra.Casillero.Numero));
            Mapper.CreateMap<MicroMuestrasPorCasilleroDto, MicroMuestrasPorCasillero>();
        }
    }
}
