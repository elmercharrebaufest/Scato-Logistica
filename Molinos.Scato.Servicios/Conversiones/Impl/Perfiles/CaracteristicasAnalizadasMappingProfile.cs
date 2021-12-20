using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CaracteristicasAnalizadasMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CaracteristicasAnalizadasMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CaracteristicasAnalizadas, CaracteristicasAnalizadasDto>();
            Mapper.CreateMap<CaracteristicasAnalizadasDto, CaracteristicasAnalizadas>();
        }
    }
}
