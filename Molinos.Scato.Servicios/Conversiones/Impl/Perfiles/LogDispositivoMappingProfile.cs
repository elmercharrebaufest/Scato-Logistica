using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LogDispositivoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LogDispositivoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LogDispositivo, LogDispositivoDto>();
            Mapper.CreateMap<LogDispositivoDto, LogDispositivo>();
        }
    }
}
