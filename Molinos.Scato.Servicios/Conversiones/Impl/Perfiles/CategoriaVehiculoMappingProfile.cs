using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CategoriaVehiculoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CategoriaVehiculoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CategoriaVehiculo, CategoriaVehiculoDto>();
            Mapper.CreateMap<CategoriaVehiculoDto, CategoriaVehiculo>();
        }
    }
}
