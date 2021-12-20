using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System.Linq;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpGaritaSalidaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpGaritaSalidaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpGaritaSalida, ImpGaritaSalidaDto>()
                .ForMember(x => x.Hidraulicas, mat => mat.MapFrom(m => m.Hidraulicas.Split(','))); ;
            Mapper.CreateMap<ImpGaritaSalidaDto, ImpGaritaSalida>()
                .ForMember(x => x.Hidraulicas, mat => mat.MapFrom(m => m.Hidraulicas.Aggregate((a, b) => a + ',' + b)));
        }
    }
}