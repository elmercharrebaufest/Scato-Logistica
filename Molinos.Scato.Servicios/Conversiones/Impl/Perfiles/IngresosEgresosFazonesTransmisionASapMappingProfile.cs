using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class IngresosEgresosFazonesTransmisionASapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "IngresosEgresosFazonesTransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<IngresosEgresosFazonesTransmisionASap, IngresosEgresosFazones>()
                .ForMember(x => x.Almacen, mat => mat.MapFrom(m => m.Almacen))
                .ForMember(x => x.Cantidad, mat => mat.MapFrom(m => m.PesoNeto))
                .ForMember(x => x.DocLegal, mat => mat.MapFrom(m => m.NumeroDocumento))
                .ForMember(x => x.FechaIng, mat => mat.MapFrom(m => m.FechaIngreso))
                .ForMember(x => x.Km, mat => mat.MapFrom(m => m.Km.HasValue ? m.Km : 0))
                .ForMember(x => x.KmSpecified, mat => mat.MapFrom(m => m.Km.HasValue))
                .ForMember(x => x.LocalidadOrig, mat => mat.MapFrom(m => m.Localidad))
                .ForMember(x => x.Procedencia, mat => mat.MapFrom(m => m.Centro))
                .ForMember(x => x.Destino, mat => mat.MapFrom(m => m.Cliente))
                .ForMember(x => x.Material, mat => mat.MapFrom(m => m.Material))
                .ForMember(x => x.Patente, mat => mat.MapFrom(m => m.Patente))
                .ForMember(x => x.ProvinciaOrig, mat => mat.MapFrom(m => m.Provincia))
                .ForMember(x => x.TipoMov, mat => mat.MapFrom(m => m.TipoMovimiento))
                .ForMember(x => x.Transportista, mat => mat.MapFrom(m => m.Transportista))
                .ForMember(x => x.UniMedCant, mat => mat.MapFrom(m => m.UnidadMedida))
                .ForMember(x => x.NombreChofer, mat => mat.MapFrom(m => m.NombreChofer))
                .ForMember(x => x.NroDocumento, mat => mat.MapFrom(m => m.NumeroDocumentoChofer))
                .ForMember(x => x.TipoDoc, mat => mat.MapFrom(m => m.TipoDocumentoChofer))
                .ForMember(x => x.Patente2, mat => mat.MapFrom(m => m.PatenteAcoplado))
                .ForMember(x => x.IM_NUM_SCATO, mat => mat.MapFrom(m => m.RecorridoId))
                ;
            Mapper.CreateMap<IngresosEgresosFazones, IngresosEgresosFazonesTransmisionASap>()
                .ForMember(x => x.Almacen, mat => mat.MapFrom(m => m.Almacen))
                .ForMember(x => x.PesoNeto, mat => mat.MapFrom(m => m.Cantidad))
                .ForMember(x => x.NumeroDocumento, mat => mat.MapFrom(m => m.DocLegal))
                .ForMember(x => x.FechaIngreso, mat => mat.MapFrom(m => m.FechaIng))
                .ForMember(x => x.Km, mat => mat.MapFrom(m => m.KmSpecified ? m.Km : (decimal?)null))
                .ForMember(x => x.Localidad, mat => mat.MapFrom(m => m.LocalidadOrig))
                .ForMember(x => x.Centro, mat => mat.MapFrom(m => m.Procedencia))
                .ForMember(x => x.Cliente, mat => mat.MapFrom(m => m.Destino))
                .ForMember(x => x.Material, mat => mat.MapFrom(m => m.Material))
                .ForMember(x => x.Patente, mat => mat.MapFrom(m => m.Patente))
                .ForMember(x => x.Provincia, mat => mat.MapFrom(m => m.ProvinciaOrig))
                .ForMember(x => x.TipoMovimiento, mat => mat.MapFrom(m => m.TipoMov))
                .ForMember(x => x.Transportista, mat => mat.MapFrom(m => m.Transportista))
                .ForMember(x => x.UnidadMedida, mat => mat.MapFrom(m => m.UniMedCant))
                .ForMember(x => x.NombreChofer, mat => mat.MapFrom(m => m.NombreChofer))
                .ForMember(x => x.NumeroDocumentoChofer, mat => mat.MapFrom(m => m.NroDocumento))
                .ForMember(x => x.TipoDocumentoChofer, mat => mat.MapFrom(m => m.TipoDoc))
                .ForMember(x => x.PatenteAcoplado, mat => mat.MapFrom(m => m.Patente2))
                .ForMember(x => x.RecorridoId, mat => mat.MapFrom(m => m.IM_NUM_SCATO))
                ;
        }
    }
}