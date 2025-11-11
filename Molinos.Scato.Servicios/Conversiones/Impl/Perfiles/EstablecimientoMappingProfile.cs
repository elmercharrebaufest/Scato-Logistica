using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EstablecimientoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EstablecimientoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Establecimiento, EstablecimientoDto>()
                  .ForMember(t => t.Localidad, f => f.MapFrom(r => r.Localidad.Descripcion))
                  .ForMember(t => t.LocalidadCodigoAfip, f => f.MapFrom(r => r.Localidad.CodigoAfip))
                  .ForMember(t => t.LocalidadId, f => f.MapFrom(r => r.Localidad.Id))
                  .ForMember(t => t.Provincia, f => f.MapFrom(r => r.Provincia.Descripcion))
                  .ForMember(t => t.Proveedor, f => f.MapFrom(r => r.Proveedor.Descripcion))
                  .ForMember(t => t.ProvinciaId, f => f.MapFrom(r => r.Provincia.Id))
                  .ForMember(t => t.EsSojaEPA, f => f.MapFrom(r => r.EPA))
                  .ForMember(t => t.ComercialId, f => f.MapFrom(r => r.Comercial.Id))
                  .ForMember(t => t.Comercial, f => f.MapFrom(r => r.Comercial.Descripcion));

            Mapper.CreateMap<EstablecimientoDto, Establecimiento>()
                  .ForMember(f => f.EPA, t => t.MapFrom(r => r.EsSojaEPA));

            Mapper.CreateMap<Establecimiento, ProveedorRENSPADto>();
        }
    }
}