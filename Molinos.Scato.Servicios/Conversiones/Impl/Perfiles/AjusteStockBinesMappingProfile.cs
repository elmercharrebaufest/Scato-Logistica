using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AjusteStockBinesMappingProfile : Profile
    {
        public override string ProfileName
        {
            get
            {
                return "AjusteYStockBinesMappingProfile";
            }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<AjusteStockBines, AjusteStockBinesDto>()
                  .ForMember(x => x.CentroId, cenB => cenB.MapFrom(ajuste => ajuste.Centro.Id))
                  .ForMember(x => x.CentroDescripcion,cenB => cenB.MapFrom(ajuste => ajuste.Centro.Descripcion))
                  .ForMember(x => x.MaterialId, TBin => TBin.MapFrom(ajuste => ajuste.Material.Id))
                  .ForMember(x => x.MaterialDescripcion, TBin => TBin.MapFrom(ajuste => ajuste.Material.Descripcion))
                  .ForMember(x => x.ProveedorId, Vp => Vp.MapFrom(ajuste => ajuste.Proveedor.Id))
                  .ForMember(x => x.ProveedorDescripcion,Vp => Vp.MapFrom(ajuste => ajuste.Proveedor.Descripcion))
                  .ForMember(x => x.VinedoPropioId, Vp => Vp.MapFrom(ajuste => ajuste.VinedoPropio.Id))
                  .ForMember(x => x.VinedoPropioDescripcion,Vp => Vp.MapFrom(ajuste => ajuste.VinedoPropio.Descripcion));
            Mapper.CreateMap<AjusteStockBinesDto, AjusteStockBines>();

        }
    }
}
