using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DescargaUnidadItemMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DescargaUnidadItemMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DescargaUnidadItem, DescargaUnidadItemDto>()
                  .ForMember(t => t.Almacen, f => f.MapFrom(r => r.Almacen.Descripcion))
                  .ForMember(t => t.AlmacenId, f => f.MapFrom(r => r.Almacen.Id))
                  .ForMember(t => t.Material, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.MaterialCodigoSap, f => f.MapFrom(r => r.Material.CodigoSAP))
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                  .ForMember(t => t.DescargaUnidadId, f => f.MapFrom(r => r.DescargaUnidad.Id))
                  .ForMember(t => t.TaraRomaneoId, f => f.MapFrom(r => r.TaraRomaneo.Id));
            Mapper.CreateMap<DescargaUnidadItemDto, DescargaUnidadItem>();
        }
    }
}