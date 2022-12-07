using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ConfiguracionCalleHidraulicaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ConfiguracionCalleHidraulicaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ConfiguracionCalleHidraulica, ConfiguracionCalleHidraulicaDto>()
                .ForMember(t => t.CalleId, f => f.MapFrom(r => r.Calle.Id))
                .ForMember(t => t.CalleNombre, f => f.MapFrom(r => r.Calle.Nombre));
            Mapper.CreateMap<ConfiguracionCalleHidraulicaDto, ConfiguracionCalleHidraulica>();
        }
    }
}