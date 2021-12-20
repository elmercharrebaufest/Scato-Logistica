using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CuposOtorgadosMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CuposOtorgadosMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CargaDeCupo, CargaDeCupoDto>();
            Mapper.CreateMap<CargaDeCupoDto, CargaDeCupo>();
        }
    }
}