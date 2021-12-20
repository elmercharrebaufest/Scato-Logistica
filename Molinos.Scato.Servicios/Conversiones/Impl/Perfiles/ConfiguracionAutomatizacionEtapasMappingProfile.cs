using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ConfiguracionAutomatizacionEtapasMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ConfiguracionAutomatizacionEtapasMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<ConfiguracionAutomatizacionEtapas, ConfiguracionAutomatizacionEtapasDto>();

            Mapper.CreateMap<ConfiguracionAutomatizacionEtapasDto, ConfiguracionAutomatizacionEtapas>();
        }
    }
}
