using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CaladoEnPlantaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CaladoEnPlantaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CaladoEnPlanta, CaladoEnPlantaDto>();
            Mapper.CreateMap<CaladoEnPlantaDto, CaladoEnPlanta>();
        }
    }
}