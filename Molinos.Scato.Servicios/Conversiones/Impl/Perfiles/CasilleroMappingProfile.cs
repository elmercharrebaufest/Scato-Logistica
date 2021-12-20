using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CasilleroMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CasilleroMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Casillero, CasilleroDto>()
                .ForMember(c => c.CentroId, x => x.MapFrom(y => y.Centro.Id));
            Mapper.CreateMap<CasilleroDto, Casillero>();
        }
    }
}