using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LogValidacionAccesoStopRespuestaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LogValidacionAccesoStopRespuestaMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<LogValidacionAccesoStopRespuesta, LogValidacionAccesoStopRespuestaDto>();
            Mapper.CreateMap<LogValidacionAccesoStopRespuestaDto, LogValidacionAccesoStopRespuesta>();
        }
    }
}
