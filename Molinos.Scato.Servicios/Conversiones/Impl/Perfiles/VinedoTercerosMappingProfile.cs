using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class VinedoTercerosMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VinedoTercerosMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<VinedoTerceros, VinedoTercerosDto>()
                .ForMember(t => t.CUITTitular, f => f.MapFrom(r => r.CUIT_Titular))
                .ForMember(t => t.Proveedor, f => f.MapFrom(r => r.Proveedor.Descripcion))
                .ForMember(t => t.ProveedorId, f => f.MapFrom(r => r.Proveedor.Id))
                .ForMember(t => t.Descripcion, f => f.MapFrom(r => r.Descripcion))
                .ForMember(t => t.IngresosBrutos, f => f.MapFrom(r => r.IngresosBrutos))
                .ForMember(t => t.NumeroINV, f => f.MapFrom(r => r.NumeroINV))
                .ForMember(t => t.CUITProveedor, f => f.MapFrom(r => r.Proveedor.Cuil))
                .ForMember(t => t.esProveedorPR, f => f.MapFrom(r => r.Proveedor.PR))
                .ForMember(t => t.SubZona, f => f.MapFrom(r => r.SubZona.Descripcion))
                .ForMember(t => t.SubZonaId, f => f.MapFrom(r => r.SubZona.Id))
                .ForMember(t => t.Zona, f => f.MapFrom(r => r.SubZona.Zona.Descripcion))
                .ForMember(t => t.ZonaId, f => f.MapFrom(r => r.SubZona.Zona.Id))
                ;
            Mapper.CreateMap<VinedoTercerosDto, VinedoTerceros>()
                .ForMember(t => t.CUIT_Titular, f => f.MapFrom(r => r.CUITTitular))
                .ForMember(t => t.Descripcion, f => f.MapFrom(r => r.Descripcion))
                .ForMember(t => t.IngresosBrutos, f => f.MapFrom(r => r.IngresosBrutos))
                .ForMember(t => t.NumeroINV, f => f.MapFrom(r => r.NumeroINV))
                .ForMember(t => t.Calidad, f => f.MapFrom(r => r.Calidad))
                ;
        }
    }
}
