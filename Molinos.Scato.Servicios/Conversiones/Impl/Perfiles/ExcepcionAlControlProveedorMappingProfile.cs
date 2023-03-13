using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ExcepcionAlControlProveedorMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ExcepcionAlControlProveedorMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ExcepcionAlControlProveedor, ExcepcionAlControlProveedorDto>()
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                  .ForMember(t => t.MaterialDesc, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.RazonSocial, f => f.MapFrom(r => r.Proveedor.RazonSocial))
                  .ForMember(t => t.ProveedorId, f => f.MapFrom(r => r.Proveedor.Id))
                  .ForMember(t => t.CentroId, f => f.MapFrom(r => r.Centro.Id))
                  .ForMember(t => t.CentroNombre, f => f.MapFrom(r => r.Centro.Descripcion))
                  .ForMember(t => t.TipoDestino, f => f.MapFrom(r => r.TipoDestino))
                  .ForMember(t => t.CentroDestinoId, f => f.MapFrom(r => r.CentroDestino.Id))
                  .ForMember(t => t.ClienteDestinoId, f => f.MapFrom(r => r.ClienteDestino.Id))
                  .ForMember(t => t.DestinoNombre, f => f.MapFrom(r => r.CentroDestino != null ? r.CentroDestino.Descripcion : r.ClienteDestino != null ? r.ClienteDestino.Descripcion : ""));
            Mapper.CreateMap<ExcepcionAlControlProveedorDto, ExcepcionAlControlProveedor>();
        }
    }
}