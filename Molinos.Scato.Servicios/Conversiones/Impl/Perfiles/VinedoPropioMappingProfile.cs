using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class VinedoPropioMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VinedoPropioMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<VinedoPropio, VinedoPropioDto>()
                .ForMember(t => t.Cuarteles, f => f.MapFrom(r => r.Cuarteles))
                .ForMember(t => t.Descripcion, f => f.MapFrom(r => r.Descripcion))
                .ForMember(t => t.IngresosBrutos, f => f.MapFrom(r => r.IngresosBrutos))
                .ForMember(t => t.NumeroINV, f => f.MapFrom(r => r.NumeroINV))
                .ForMember(t => t.SubZona, f => f.MapFrom(r => r.SubZona.Descripcion))
                .ForMember(t => t.SubZonaId, f => f.MapFrom(r => r.SubZona.Id))
                .ForMember(t => t.Zona, f => f.MapFrom(r => r.SubZona.Zona.Descripcion))
                .ForMember(t => t.ZonaId, f => f.MapFrom(r => r.SubZona.Zona.Id))
                ;
            Mapper.CreateMap<VinedoPropioDto, VinedoPropio>()
                .ForMember(t => t.Cuarteles, f => f.MapFrom(r => r.Cuarteles))
                .ForMember(t => t.Descripcion, f => f.MapFrom(r => r.Descripcion))
                .ForMember(t => t.IngresosBrutos, f => f.MapFrom(r => r.IngresosBrutos))
                .ForMember(t => t.NumeroINV, f => f.MapFrom(r => r.NumeroINV))
                .ForMember(t => t.Calidad, f => f.MapFrom(r => r.Calidad))
                ;
        }
    }
}
