using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class GraficoDePlantaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "GraficoDePlantaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<GraficoDePlanta, GraficoDePlantaDto>();
            Mapper.CreateMap<GraficoDePlantaDto, GraficoDePlanta>();
        }
    }
}