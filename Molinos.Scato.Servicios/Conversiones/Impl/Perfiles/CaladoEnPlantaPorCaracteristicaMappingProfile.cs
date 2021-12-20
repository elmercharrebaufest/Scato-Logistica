using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CaladoEnPlantaPorCaracteristicaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CaladoEnPlantaPorCaracteristicaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CaladoEnPlantaPorCaracteristica, CaladoEnPlantaPorCaracteristicaDto>();
            Mapper.CreateMap<CaladoEnPlantaPorCaracteristicaDto, CaladoEnPlantaPorCaracteristica>();
        }
    }
}