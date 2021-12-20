using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CoordinadorPuertoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CoordinadorPuertoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CoordinadorPuerto, CoordinadorPuertoDto>();
            Mapper.CreateMap<CoordinadorPuertoDto, CoordinadorPuerto>();
        }
    }
}