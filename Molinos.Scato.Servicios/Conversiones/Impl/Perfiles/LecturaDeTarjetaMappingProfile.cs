using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LecturaDeTarjetaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LecturaDeTarjetaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LecturaDeTarjeta, LecturaDeTarjetaDto>();
            Mapper.CreateMap<LecturaDeTarjetaDto, LecturaDeTarjeta>();
        }
    }
}