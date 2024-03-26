using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AutomatismoNoGranoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AutomatismoNoGranoMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<AutomatismoNoGrano, AutomatismoNoGranoDto>()
                 .ForMember(x => x.CantidadCamiones, y => y.MapFrom(x => x.CallePlanta.CantidadDeCamiones));
            Mapper.CreateMap<AutomatismoNoGranoDto, AutomatismoNoGrano>();
        }
    }
}
