using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MovimientoDeBinesMappingProfile : Profile
    {
        public override string ProfileName
        {
            get
            {
                return "MovimientoDeBinesMappingProfile";
            }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<MovimientoDeBines, MovimientoDeBinesDto>()
                  .ForMember(x => x.CentroId, cenB => cenB.MapFrom(ajuste => ajuste.Centro.Id))
                  .ForMember(x => x.CentroDescripcion, cenB => cenB.MapFrom(ajuste => ajuste.Centro.Descripcion))
                  .ForMember(x => x.MaterialId, TBin => TBin.MapFrom(ajuste => ajuste.Material.Id))
                  .ForMember(x => x.MaterialDescripcion, TBin => TBin.MapFrom(ajuste => ajuste.Material.Descripcion))
                  .ForMember(x => x.ProveedorId, Vp => Vp.MapFrom(ajuste => ajuste.Proveedor.Id))
                  .ForMember(x => x.ProveedorDescripcion, Vp => Vp.MapFrom(ajuste => ajuste.Proveedor.Descripcion))
                  .ForMember(x => x.CentroOrigenId, cenB => cenB.MapFrom(ajuste => ajuste.CentroOrigen.Id))
                  .ForMember(x => x.CentroOrigenDescripcion, cenB => cenB.MapFrom(ajuste => ajuste.CentroOrigen.Descripcion))
                  .ForMember(x => x.VinedoPropioId, Vp => Vp.MapFrom(ajuste => ajuste.VinedoPropio.Id))
                  .ForMember(x => x.VinedoPropioDescripcion, Vp => Vp.MapFrom(ajuste => ajuste.VinedoPropio.Descripcion));
            Mapper.CreateMap<MovimientoDeBinesDto, MovimientoDeBines>();

        }
    }
}
