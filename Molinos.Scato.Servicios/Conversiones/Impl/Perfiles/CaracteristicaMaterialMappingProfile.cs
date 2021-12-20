using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CaracteristicaMaterialMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CaracteristicaMaterialMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CaracteristicaMaterial, CaracteristicaMaterialDto>();
            Mapper.CreateMap<CaracteristicaMaterialDto, CaracteristicaMaterial>();
        }
    }
}