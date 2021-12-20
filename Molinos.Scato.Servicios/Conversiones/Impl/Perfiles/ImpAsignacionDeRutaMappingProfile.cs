using System.Linq;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpAsignacionDeRutaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpAsignacionDeRutaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpAsignacionDeRuta, ImpAsignacionDeRutaDto>()
            .ForMember(x => x.Hidraulicas, mat => mat.MapFrom(m => m.Hidraulicas.Split(',')));
            Mapper.CreateMap<ImpAsignacionDeRutaDto, ImpAsignacionDeRuta>()
            .ForMember(x => x.Hidraulicas, mat => mat.MapFrom(m => m.Hidraulicas.Aggregate((a, b) => a + ',' + b)));
        }
    }
}
