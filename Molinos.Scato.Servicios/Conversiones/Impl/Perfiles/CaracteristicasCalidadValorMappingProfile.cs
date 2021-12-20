using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CaracteristicasCalidadValorMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CaracteristicasCalidadValorMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CaracteristicasCalidadValor, CaracteristicasCalidadValorDto>();
            Mapper.CreateMap<CaracteristicasCalidadValorDto, CaracteristicasCalidadValor>();
        }
    }
}