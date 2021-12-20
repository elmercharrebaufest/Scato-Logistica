using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RemitoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RemitoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Remito, RemitoDto>()
                  .ForMember(t => t.Transportista, f => f.MapFrom(r => r.Transportista.RazonSocial))
                  .ForMember(t => t.TransportistaCuit, f => f.MapFrom(r => r.Transportista.Cuit))
                  .ForMember(t => t.TransportistaId, f => f.MapFrom(r => r.Transportista.Id))
                  .ForMember(t => t.TipoComercial, f => f.MapFrom(r => r.TipoComercial.Descripcion))
                  .ForMember(t => t.TipoComercialCodigoSap, f => f.MapFrom(r => r.TipoComercial.CodigoSap))
                  .ForMember(t => t.TipoComercialId, f => f.MapFrom(r => r.TipoComercial.Id))
                  .ForMember(t => t.Material, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                  .ForMember(t => t.MaterialPideContrato, f => f.MapFrom(r => r.Material.Contrato))
                  .ForMember(t => t.Origen, f => f.MapFrom(r => r.CentroOrigen != null ? r.CentroOrigen.Descripcion : r.ProveedorOrigen.RazonSocial))
                  .ForMember(t => t.OrigenId, f => f.MapFrom(r => r.CentroOrigen != null ? r.CentroOrigen.Id : r.ProveedorOrigen.Id))
                  .ForMember(t => t.OrigenCodigoSap, f => f.MapFrom(r => r.CentroOrigen != null ? r.CentroOrigen.CodigoSAP : r.ProveedorOrigen.CodigoSap))
                  .ForMember(t => t.OrigenMail, f => f.MapFrom(r => r.CentroOrigen != null ? string.Empty : r.ProveedorOrigen.EnvioAutomaticoMail && !string.IsNullOrEmpty(r.ProveedorOrigen.Mail) ? r.ProveedorOrigen.Mail : string.Empty))
                  .ForMember(t => t.OrigenEnviaMailEnAnalisis, f => f.MapFrom(r => r.CentroOrigen == null && r.ProveedorOrigen.Analisis))
                  .ForMember(t => t.OrigenEnviaMailEnPesada, f => f.MapFrom(r => r.CentroOrigen == null && r.ProveedorOrigen.Pesada))
                  .ForMember(t => t.Remito, f => f.MapFrom(r => r.OrdenRemito))
                  .ForMember(t => t.EsRemitoProveedor, f => f.MapFrom(r => r.ProveedorOrigen != null))
                  .ForMember(t => t.DocLegalRemito, f => f.MapFrom(r => r.DocLegalRemito))
                  .ForMember(t => t.RecorridoId, f => f.MapFrom(r => r.Recorrido.Id))
                  .ForMember(t => t.MaterialCodigoSap, f => f.MapFrom(r => r.Material.CodigoSAP))
                  .ForMember(t => t.Procedencia, f => f.MapFrom(r => r.Procedencia.Descripcion))
                  .ForMember(t => t.ProcedenciaCodigoSap, f => f.MapFrom(r => r.Procedencia.CodigoAfip))
                  .ForMember(t => t.ProcedenciaId, f => f.MapFrom(r => r.Procedencia.Id))
                  .ForMember(t => t.ProvinciaCodigoSap, f => f.MapFrom(r => r.Procedencia.Provincia.CodigoAfip))
                  .ForMember(t => t.TipoVehiculo, f => f.MapFrom(r => r.Recorrido.TipoVehiculo));
            Mapper.CreateMap<RemitoDto, Remito>();

        }
    }
}
