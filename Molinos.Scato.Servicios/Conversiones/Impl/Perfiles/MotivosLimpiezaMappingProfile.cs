using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MotivosLimpiezaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MotivosLimpiezaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MotivosLimpieza, MotivosLimpiezaDto>();
            Mapper.CreateMap<MotivosLimpiezaDto, MotivosLimpieza>();
        }
    }
}