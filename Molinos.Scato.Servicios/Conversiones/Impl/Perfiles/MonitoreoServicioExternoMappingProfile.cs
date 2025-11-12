using AutoMapper;
using Molinos.Scato.Dominio.Dto.HealthCheck;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MonitoreoServicioExternoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ExternalServiceMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<MonitoreoServicioExterno, MonitoreoServicioExternoDto>();
            Mapper.CreateMap<MonitoreoServicioExternoDto, MonitoreoServicioExterno>();
        }
    }
}
