using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AgenciaMaritimaPuertoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AgenciaMaritimaPuertoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AgenciaMaritimaPuerto, AgenciaMaritimaPuertoDto>();
            Mapper.CreateMap<AgenciaMaritimaPuertoDto, AgenciaMaritimaPuerto>();
        }
    }
}