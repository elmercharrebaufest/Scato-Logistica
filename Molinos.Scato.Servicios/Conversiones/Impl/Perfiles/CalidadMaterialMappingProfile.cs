using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CalidadMaterialMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CalidadMaterialMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CalidadMaterial, CalidadMaterialDto>()
                  .ForMember(t => t.MaterialPorCentroId, f => f.MapFrom(r => r.MaterialPorCentro.Id))
                  .ForMember(t => t.Material, f => f.MapFrom(r => r.MaterialPorCentro.Material.Descripcion))
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.MaterialPorCentro.Material.Id));
            Mapper.CreateMap<CalidadMaterialDto, CalidadMaterial>();
        }
    }
}