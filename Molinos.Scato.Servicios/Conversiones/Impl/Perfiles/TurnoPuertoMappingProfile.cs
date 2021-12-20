using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TurnoPuertoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TurnoPuertoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TurnoPuerto, TurnoPuertoDto>();
            Mapper.CreateMap<TurnoPuertoDto, TurnoPuerto>();
        }
    }
}