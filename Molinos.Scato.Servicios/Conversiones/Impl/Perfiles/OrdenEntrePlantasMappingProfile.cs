using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class OrdenEntrePlantasMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "OrdenEntrePlantasMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<OrdenEntrePlantas, OrdenEntrePlantasDto>()
                  .ForMember(x => x.MaterialId, c => c.MapFrom(o => o.Material.Id))
                  .ForMember(x => x.Material, c => c.MapFrom(o => o.Material.Descripcion))
                  .ForMember(x => x.TipoComercialId, c => c.MapFrom(o => o.TipoComercial.Id))
                  .ForMember(x => x.TipoComercial, c => c.MapFrom(o => o.TipoComercial.Descripcion))
                  .ForMember(x => x.TransportistaId, c => c.MapFrom(o => o.Transportista.Id))
                  .ForMember(x => x.Transportista, c => c.MapFrom(o => o.Transportista.RazonSocial))
                  .ForMember(x => x.TransportistaCuit, c => c.MapFrom(o => o.Transportista.Cuit))
                  .ForMember(x => x.TransportistaId, c => c.MapFrom(o => o.Transportista.Id))
                  .ForMember(x => x.MaterialCodigoSap, c => c.MapFrom(o => o.Material.CodigoSAP))
                  .ForMember(x => x.CentroDestinoId, c => c.MapFrom(o => o.CentroDestino.Id))
                  .ForMember(x => x.CentroDestino, c => c.MapFrom(o => o.CentroDestino.Descripcion))
                  .ForMember(x => x.CentroDestinoCodigoSap, c => c.MapFrom(o => o.CentroDestino.CodigoSAP))
                  .ForMember(x => x.CentroDestinoCuit, c => c.MapFrom(o => o.CentroDestino.Cuit))
                  .ForMember(x => x.CentroDestinoLocalidad, c => c.MapFrom(o => o.CentroDestino.Localidad.Descripcion))
                  .ForMember(x => x.CentroDestinoDireccion, c => c.MapFrom(o => o.CentroDestino.Direccion))
                  .ForMember(x => x.CentroDestinoProvincia, c => c.MapFrom(o => o.CentroDestino.Provincia.Descripcion))
                  .ForMember(x => x.RecorridoId, c => c.MapFrom(o => o.Recorrido.Id))
                  .ForMember(x => x.PesoNetoOrigen, c => c.MapFrom(o => (o.Recorrido.PesoBrutoOrigen - o.Recorrido.PesoTaraOrigen)))
                  .ForMember(x => x.DocumentoInternoSap, c => c.MapFrom(o => o.Recorrido.DocumentoInternoSap))
                  .ForMember(t => t.MaterialPideLote, f => f.MapFrom(r => r.Material.Lote))
                  .ForMember(t => t.KmARecorrer, f => f.MapFrom(r => r.KmRecorrer.HasValue ? r.KmRecorrer.ToString() : ""))
                  .ForMember(x => x.TipoVehiculo, c => c.MapFrom(o => o.Recorrido.TipoVehiculo))
                  ;
            Mapper.CreateMap<OrdenEntrePlantasDto, OrdenEntrePlantas>();

        }
    }
}
