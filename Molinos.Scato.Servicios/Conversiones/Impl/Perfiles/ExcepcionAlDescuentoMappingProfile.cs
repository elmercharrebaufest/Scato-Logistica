using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ExcepcionAlDescuentoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ExcepcionAlDescuentoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ExcepcionAlDescuento, ExcepcionAlDescuentoDto>()
                .ForMember(t => t.ProveedorId, f => f.MapFrom(r => r.Proveedor.Id))
                .ForMember(t => t.ProveedorDescripcion, f => f.MapFrom(r => r.Proveedor.RazonSocial))
                .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.CaracteristicaDeCalidad.MaterialPorCentro.Material.Id))
                .ForMember(t => t.MaterialDescripcion, f => f.MapFrom(r => r.CaracteristicaDeCalidad.MaterialPorCentro.Material.Descripcion))
                .ForMember(t => t.CaracteristicaDeCalidadId, f => f.MapFrom(r => r.CaracteristicaDeCalidad.Id))
                .ForMember(t => t.CaracteristicaDeCalidadDescripcion, f => f.MapFrom(r => r.CaracteristicaDeCalidad.DescripcionCorta))
                .ForMember(t => t.CamaraId, f => f.MapFrom(r => r.Camara.Id));
            Mapper.CreateMap<ExcepcionAlDescuentoDto, ExcepcionAlDescuento>();
        }
    }
}