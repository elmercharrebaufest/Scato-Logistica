using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class HumedimetroMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "HumedimetroMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Humedimetro, HumedimetroDto>()
                .ForMember(x => x.CentroId, h => h.MapFrom(humedimetro => humedimetro.Centro.Id)); ;
            Mapper.CreateMap<HumedimetroDto, Humedimetro>();
        }
    }
}
