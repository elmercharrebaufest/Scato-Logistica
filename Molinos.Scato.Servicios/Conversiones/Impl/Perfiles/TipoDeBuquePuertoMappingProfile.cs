using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TipoDeBuquePuertoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TipoDeBuquePuertoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TipoDeBuquePuerto, TipoDeBuquePuertoDto>();
            Mapper.CreateMap<TipoDeBuquePuertoDto, TipoDeBuquePuerto>();
        }
    }
}