using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class OrdenCargaInternaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "OrdenCargaInternaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<OrdenCargaInterna, OrdenCargaInternaDto>()
                  .ForMember(x => x.MaterialId, c => c.MapFrom(o => o.Material.Id))
                  .ForMember(x => x.MaterialDesc, c => c.MapFrom(o => o.Material.Descripcion))
                  .ForMember(x => x.TipoComercialId, c => c.MapFrom(o => o.TipoComercial.Id))
                  .ForMember(x => x.TipoComercialDesc, c => c.MapFrom(o => o.TipoComercial.Descripcion))
                  .ForMember(x => x.TransportistaId, c => c.MapFrom(o => o.Transportista.Id))
                  .ForMember(x => x.Transportista, c => c.MapFrom(o => o.Transportista.RazonSocial))
                  .ForMember(x => x.DestinoId, c => c.MapFrom(o => o.Destino.Id))
                  .ForMember(x => x.Destino, c => c.MapFrom(o => o.Destino.Descripcion))
                  .ForMember(x => x.ClienteDireccion, c => c.MapFrom(o => o.Destino.Direccion))
                  .ForMember(x => x.ClienteLocalidad, c => c.MapFrom(o => o.Destino.Localidad))
                  .ForMember(x => x.ClienteProvincia, c => c.MapFrom(o => o.Destino.Provincia))

                  .ForMember(x => x.PatenteAcoplado, c => c.MapFrom(o => o.PatenteAcoplado != null ? o.PatenteAcoplado : ""))

                  .ForMember(x => x.ClienteDescripcion, c => c.MapFrom(o => o.Destino.Descripcion))
                  .ForMember(x => x.ClienteDireccion, c => c.MapFrom(o => o.Destino.Direccion))
                  .ForMember(x => x.ClienteLocalidad, c => c.MapFrom(o => o.Destino.Localidad))

                  .ForMember(x => x.ClienteCuit, c => c.MapFrom(o => o.Destino.Cuit))
                  .ForMember(x => x.ClienteCodigoSap, c => c.MapFrom(o => o.Destino.CodigoSap))
                  .ForMember(x => x.TransportistaCuit, c => c.MapFrom(o => o.Transportista.Cuit))
                  .ForMember(x => x.TransportistaDomicilio, c => c.MapFrom(o => o.Transportista.Domicilio))
                  .ForMember(x => x.TransportistaLocalidad, c => c.MapFrom(o => o.Transportista.Localidad.Descripcion))
                  .ForMember(x => x.MaterialUnidadMedida, c => c.MapFrom(o => o.Material.UnidadDeMedidad))
                  .ForMember(x => x.LocalidadDestinoId, c => c.MapFrom(o => o.LocalidadDestino.Id))
                  .ForMember(x => x.LocalidadDestinoDescripcion, c => c.MapFrom(o => o.LocalidadDestino.Descripcion))
                  .ForMember(x => x.TipoVehiculo, c => c.MapFrom(o => o.Recorrido.TipoVehiculo))
                  .ForMember(x => x.PagadorFleteId, c => c.MapFrom(o => o.PagadorFlete.Id))
                  .ForMember(x => x.PagadorFlete, c => c.MapFrom(o => o.PagadorFlete.Descripcion));

            Mapper.CreateMap<OrdenCargaInternaDto, OrdenCargaInterna>();
            
        }
    }
}
