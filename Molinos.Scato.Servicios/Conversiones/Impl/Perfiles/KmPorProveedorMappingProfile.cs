using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class KmPorProveedorMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "KmPorProveedorMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<KmPorProveedor, KmPorProveedorDto>()
                .ForMember(t => t.CentroId, f => f.MapFrom(r => r.Centro.Id))
                .ForMember(t => t.CentroDescripcion, f => f.MapFrom(r => r.Centro.Descripcion))
                .ForMember(t => t.ClienteId, f => f.MapFrom(r => r.Cliente.Id))
                .ForMember(t => t.ClienteDescripcion, f => f.MapFrom(r => r.Cliente.Descripcion))
                .ForMember(t => t.ProvinciaId, f => f.MapFrom(r => r.Localidad.Provincia.Id))
                .ForMember(t => t.ProvinciaDescripcion, f => f.MapFrom(r => r.Localidad.Provincia.Descripcion))
                .ForMember(t => t.LocalidadId, f => f.MapFrom(r => r.Localidad.Id))
                .ForMember(t => t.LocalidadDescripcion, f => f.MapFrom(r => r.Localidad.Descripcion))
                .ForMember(t => t.KmARecorrer, f => f.MapFrom(r => r.KmARecorrer));

            Mapper.CreateMap<KmPorProveedorDto, KmPorProveedor>();
        }
    }
}