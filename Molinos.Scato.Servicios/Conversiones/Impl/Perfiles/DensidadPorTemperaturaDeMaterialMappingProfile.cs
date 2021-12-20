using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DensidadPorTemperaturaDeMaterialMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DensidadPorTemperaturaDeMaterialMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DensidadPorTemperaturaDeMaterial, DensidadPorTemperaturaDeMaterialDto>();
            Mapper.CreateMap<DensidadPorTemperaturaDeMaterialDto, DensidadPorTemperaturaDeMaterial>();
        }
    }
}
