using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LogActividadHistoricoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LogActividadHistoricoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LogActividadHistorico, LogActividadHistoricoDto>();
            Mapper.CreateMap<LogActividadHistoricoDto, LogActividadHistorico>();
        }
    }
}
