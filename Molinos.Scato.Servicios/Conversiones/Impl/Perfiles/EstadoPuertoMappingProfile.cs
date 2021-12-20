using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EstadoPuertoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EstadoPuertoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<EstadoPuerto, EstadoPuertoDto>();
            Mapper.CreateMap<EstadoPuertoDto, EstadoPuerto>();
        }
    }
}