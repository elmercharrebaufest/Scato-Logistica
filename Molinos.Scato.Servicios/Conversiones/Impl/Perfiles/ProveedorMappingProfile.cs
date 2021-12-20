using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ProveedorMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ProveedorMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Proveedor, ProveedorDto>()
                  .ForMember(f => f.PaisId, t => t.MapFrom(s => s.Pais.Id))
                  .ForMember(f => f.PaisDesc, t => t.MapFrom(s => s.Pais.Descripcion))
                  .ForMember(f => f.ProvinciaId, t => t.MapFrom(s => s.Provincia.Id))
                  .ForMember(f => f.ProvinciaDesc, t => t.MapFrom(s => s.Provincia.Descripcion))
                  .ForMember(f => f.LocalidadId, t => t.MapFrom(s => s.Localidad.Id))
                  .ForMember(f => f.LocalidadDesc, t => t.MapFrom(s => s.Localidad.Descripcion));
            Mapper.CreateMap<ProveedorDto, Proveedor>();
        }
    }
}
