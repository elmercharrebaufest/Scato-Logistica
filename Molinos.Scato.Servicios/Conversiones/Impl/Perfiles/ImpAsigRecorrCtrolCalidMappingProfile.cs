using System.Linq;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpAsigRecorrCtrolCalidMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpAsigRecorrCtrolCalidMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpAsigRecorrCtrolCalid, ImpAsigRecorrCtrolCalidDto>()
            .ForMember(x => x.Hidraulicas, mat => mat.MapFrom(m => m.Hidraulicas.Split(',')));
            Mapper.CreateMap<ImpAsigRecorrCtrolCalidDto, ImpAsigRecorrCtrolCalid>()
            .ForMember(x => x.Hidraulicas, mat => mat.MapFrom(m => m.Hidraulicas.Aggregate((a, b) => a + ',' + b)));
        }
    }
}
