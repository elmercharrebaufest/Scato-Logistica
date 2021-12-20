using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DescargaUnidadItemPedidoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DescargaUnidadItemPedidoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DescargaUnidadItemPedido, DescargaUnidadItemPedidoDto>()
            .ForMember(t => t.MaterialDescripcion, f => f.MapFrom(r => r.Material.Descripcion))
            .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id));
            Mapper.CreateMap<DescargaUnidadItemPedidoDto, DescargaUnidadItemPedido>();
        }
    }
}