using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ConversionMaterialMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ConversionMaterialMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ConversionMaterial, ConversionMaterialDto>()
                  .ForMember(t => t.MaterialDesc, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                  .ForMember(t => t.CamaraDesc, f => f.MapFrom(r => r.Camara.Descripcion))
                  .ForMember(t => t.CamaraId, f => f.MapFrom(r => r.Camara.Id));
            Mapper.CreateMap<ConversionMaterialDto, ConversionMaterial>();
        }
    }
}