using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LogActividadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LogActividadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LogActividad, LogActividadDto>();
            Mapper.CreateMap<LogActividadDto, LogActividad>();
        }
    }
}
