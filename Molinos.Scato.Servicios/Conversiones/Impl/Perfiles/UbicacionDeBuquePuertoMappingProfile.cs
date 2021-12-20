using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class UbicacionDeBuquePuertoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "UbicacionDeBuquePuertoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<UbicacionDeBuquePuerto, UbicacionDeBuquePuertoDto>();
            Mapper.CreateMap<UbicacionDeBuquePuertoDto, UbicacionDeBuquePuerto>();
        }
    }
}