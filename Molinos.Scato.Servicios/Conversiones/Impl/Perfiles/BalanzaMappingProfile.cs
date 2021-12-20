using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class BalanzaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "BalanzaMappingProfile"; }
        }
        protected override void Configure()
        {

            Mapper.CreateMap<Balanza, BalanzaDto>()
                  .ForMember(x => x.CentroId, b => b.MapFrom(balanza => balanza.Centro.Id));
            Mapper.CreateMap<BalanzaDto, Balanza>();
        }
    }
}
