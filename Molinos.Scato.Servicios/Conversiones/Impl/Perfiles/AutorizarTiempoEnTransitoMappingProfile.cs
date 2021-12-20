using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AutorizarTiempoEnTransitoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AutorizarTiempoEnTransitoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AutorizarTiempoEnTransito, AutorizacionTiempoEnTransitoDto>()
                  .ForMember(x => x.Actividad1, b => b.MapFrom(x => x.Actividad1));
            Mapper.CreateMap<AutorizacionTiempoEnTransitoDto, AutorizarTiempoEnTransito>();
        }
    }
}
