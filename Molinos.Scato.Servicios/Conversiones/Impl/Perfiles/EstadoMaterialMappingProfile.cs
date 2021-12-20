using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EstadoMaterialMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EstadoMaterialMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<EstadoMaterial, EstadoMaterialDto>();
            Mapper.CreateMap<EstadoMaterialDto, EstadoMaterial>();
        }
    }
}
