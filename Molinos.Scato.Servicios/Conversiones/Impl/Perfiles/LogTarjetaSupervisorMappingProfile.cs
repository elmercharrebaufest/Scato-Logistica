using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LogTarjetaSupervisorMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LogTarjetaSupervisorMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LogTarjetaSupervisor, LogTarjetaSupervisorDto>()
                  .ForMember(x => x.PuestoTrabajoId, f => f.MapFrom(r => r.PuestoDeTrabajo.Id))
                  .ForMember(x => x.PuestoTrabajo, f => f.MapFrom(r => r.PuestoDeTrabajo.NombrePuesto))
                  .ForMember(x => x.Hora, f => f.MapFrom(r => r.Fecha.ToShortTimeString()));
            Mapper.CreateMap<LogTarjetaSupervisorDto, LogTarjetaSupervisor>();
        }
    }
}