using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class OrdenDeDescargaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "OrdenDeDescargaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<OrdenDeDescarga, OrdenDeDescargaDto>()
                  .ForMember(t => t.Transportista, f => f.MapFrom(r => r.Transportista.RazonSocial))
                  .ForMember(t => t.TransportistaId, f => f.MapFrom(r => r.Transportista.Id))
                  .ForMember(t => t.TipoComercial, f => f.MapFrom(r => r.TipoComercial.Descripcion))
                  .ForMember(t => t.TipoComercialSentido, f => f.MapFrom(r => r.TipoComercial.Sentido))
                  .ForMember(t => t.TipoComercialId, f => f.MapFrom(r => r.TipoComercial.Id))
                  .ForMember(t => t.TipoComercialCodigoSap, f => f.MapFrom(r => r.TipoComercial.CodigoSap))                  
                  .ForMember(t => t.Proveedor, f => f.MapFrom(r => r.Proveedor.Descripcion))
                  .ForMember(t => t.ProveedorId, f => f.MapFrom(r => r.Proveedor.Id))
                  .ForMember(t => t.ProveedorCodigoSap, f => f.MapFrom(r => r.Proveedor.CodigoSap))
                  .ForMember(t => t.ProveedorCuil, f => f.MapFrom(r => r.Proveedor.Cuil))
                  .ForMember(t => t.RecorridoId, f => f.MapFrom(r => r.Recorrido.Id))
                  .ForMember(t => t.TipoVehiculo, f => f.MapFrom(r => r.Recorrido.TipoVehiculo));
            Mapper.CreateMap<OrdenDeDescargaDto, OrdenDeDescarga>();

        }
    }
}
