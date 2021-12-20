using System;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RemitoBodegaUvaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RemitoBodegaUvaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<RemitoBodegaUva, RemitoBodegaUvaDto>()
                  .ForMember(x => x.RecorridoId, cxc => cxc.MapFrom(x => x.Recorrido.Id))
                  .ForMember(x => x.TipoComercialId, cxc => cxc.MapFrom(x => x.TipoComercial.Id))
                  .ForMember(x => x.TipoComercial, cxc => cxc.MapFrom(x => x.TipoComercial.Descripcion))
                  .ForMember(x => x.TransportistaId, cxc => cxc.MapFrom(x => x.Transportista.Id))
                  .ForMember(x => x.Transportista, cxc => cxc.MapFrom(x => x.Transportista.RazonSocial))
                  .ForMember(x => x.ProveedorId, cxc => cxc.MapFrom(x => x.Proveedor.Id))
                  .ForMember(x => x.Proveedor, cxc => cxc.MapFrom(x => x.Proveedor.Descripcion))
                  .ForMember(x => x.VinedoId, cxc => cxc.MapFrom(x => x.Vinedo.Id))
                  .ForMember(x => x.Vinedo, cxc => cxc.MapFrom(x => x.Vinedo.Descripcion))
                  .ForMember(x => x.VinedoINV, cxc => cxc.MapFrom(x => x.Vinedo.NumeroINV))
                  .ForMember(x => x.VinedoCuit, cxc => cxc.MapFrom(x => x.Vinedo.VinateroCuit()))
                  .ForMember(x => x.VinedoVinatero, cxc => cxc.MapFrom(x => x.Vinedo.Vinatero()))
                  .ForMember(x => x.VinedoIIBB, cxc => cxc.MapFrom(x => x.Vinedo.IngresosBrutos))
                  .ForMember(x => x.VariedadId, cxc => cxc.MapFrom(x => x.Material.Variedad.Id))
                  .ForMember(x => x.VinedoCentroOperativo, cxc => cxc.MapFrom(x => x.Vinedo.CentroOperativoCodigoSap()))
                  .ForMember(x => x.Variedad, cxc => cxc.MapFrom(x => x.Material.Variedad.Descripcion))
                  .ForMember(x => x.TipoVehiculoBodegaId, cxc => cxc.MapFrom(x => x.TipoVehiculoBodega.Id))
                  .ForMember(x => x.TipoComercialEsParaUva, cxc => cxc.MapFrom(x => x.TipoComercial.EsParaUva))
                  .ForMember(x => x.TipoVehiculoBodega, cxc => cxc.MapFrom(x => x.TipoVehiculoBodega.Descripcion))
                  .ForMember(x => x.Material, cxc => cxc.MapFrom(x => x.Material.Descripcion))
                  .ForMember(t => t.EsPropiaPesTercerosT, f => f.MapFrom(r => r.Recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.RemitoBodegaUvaPropia ? "P" : "T"))
                  .ForMember(x => x.VinedoSubZona, cxc => cxc.MapFrom(x => x.Vinedo.SubZona.Descripcion))
                  .ForMember(x => x.VinedoZona, cxc => cxc.MapFrom(x => x.Vinedo.SubZona.Zona.Descripcion))
                  .ForMember(x => x.VinedoCalidad, cxc => cxc.MapFrom(x => x.Vinedo.Calidad))
                  .ForMember(x => x.PesoNetoBodega, cxc => cxc.MapFrom(x => x.Recorrido.PesoBruto.HasValue && x.Recorrido.PesoTaraBodega.HasValue ? (x.Recorrido.PesoBruto.Value - x.Recorrido.PesoTaraBodega.Value) : (int?)null));


            Mapper.CreateMap<RemitoBodegaUvaDto, RemitoBodegaUva>();

            Mapper.CreateMap<RemitoBodegaUva, ArchivoINVFilaDto>()
               .ForMember(t => t.SesentaSiempre, f => f.MapFrom(r => "60"))
               .ForMember(t => t.NumeroCiu, f => f.MapFrom(r => r.Recorrido.NumeroCiu))
               .ForMember(t => t.FechaEgreso, f => f.MapFrom(r => r.Recorrido.FechaEgreso.HasValue ? r.Recorrido.FechaEgreso.Value.ToString("yyyy-MM-dd") : ""))
               .ForMember(t => t.NumeroINVVinedo, f => f.MapFrom(r => r.Vinedo.NumeroINV))
               .ForMember(t => t.RazonSocialVinedo, f => f.MapFrom(r => r.Vinedo.Vinatero()))
               .ForMember(t => t.CuitVinedo, f => f.MapFrom(r => r.Vinedo.VinateroCuit()))
               .ForMember(t => t.IIBBVinedo, f => f.MapFrom(r => r.Vinedo.IngresosBrutos))
               .ForMember(t => t.PesoBruto, f => f.MapFrom(r => r.Recorrido.PesoBruto.HasValue ? r.Recorrido.PesoBruto.ToString() : ""))
               .ForMember(t => t.PesoTaraBodega, f => f.MapFrom(r => r.Recorrido.PesoTaraBodega.HasValue ? r.Recorrido.PesoTaraBodega.ToString() : ""))
               .ForMember(t => t.PesoNeto, f => f.MapFrom(r => r.Recorrido.PesoBruto.HasValue && r.Recorrido.PesoTaraBodega.HasValue ? (r.Recorrido.PesoBruto.Value - r.Recorrido.PesoTaraBodega.Value).ToString() : ""))
               .ForMember(t => t.Patente, f => f.MapFrom(r => r.Patente))
               .ForMember(t => t.ModeloCamion, f => f.MapFrom(r => r.ModeloCamion))
               .ForMember(t => t.NombreChofer, f => f.MapFrom(r => (r.Chofer.Apellido.ToUpper() + " " + r.Chofer.Nombre.ToUpper()).Truncate(20)))
               .ForMember(t => t.NumeroInvVariedadMaterial, f => f.MapFrom(r => r.Material.Variedad.NumeroINV))
               .ForMember(t => t.EsPropiaPesTercerosT, f => f.MapFrom(r => r.Recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.RemitoBodegaUvaPropia ? "P" : "T"))
               .ForMember(t => t.Campo34, f => f.MapFrom(r => "C"))
               .ForMember(t => t.CiuCorrectoALRechazadoAN, f => f.MapFrom(r => "AL"))
               .ForMember(t => t.Campo36, f => f.MapFrom(r => "0"))
               .ForMember(t => t.AñoActual, f => f.MapFrom(r => DateTime.Now.Year.ToString()))
               .ForMember(t => t.TipoVehiculo, f => f.MapFrom(r => r.TipoVehiculoBodega.DescripcionCorta))
               .ForMember(t => t.TipoCosecha, f => f.MapFrom(r => ((int)r.TipoCosecha).ToString()));
        }
    }
}
