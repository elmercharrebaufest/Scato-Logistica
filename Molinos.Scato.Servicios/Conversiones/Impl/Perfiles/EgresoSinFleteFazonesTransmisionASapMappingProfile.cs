using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EgresoSinFleteFazonesTransmisionASapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EgresoSinFleteFazonesTransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<EgresoSinFleteFasonesTransmisionASap, EgresoSinFleteFazones>()
                  .ForMember(x => x.Almacen, mat => mat.MapFrom(m => m.Almacen))
                  .ForMember(x => x.CUITCliente, mat => mat.MapFrom(m => m.CuitClienteDestinatario))
                  .ForMember(x => x.CUITD, mat => mat.MapFrom(m => m.CuitTransportista))
                  .ForMember(x => x.Cantidad, mat => mat.MapFrom(m => m.Cantidad))
                  .ForMember(x => x.Centro, mat => mat.MapFrom(m => m.Centro))
                  .ForMember(x => x.FechaCon, mat => mat.MapFrom(m => m.FechaCon))
                  .ForMember(x => x.FechaDoc, mat => mat.MapFrom(m => m.FechaDoc))
                  .ForMember(x => x.Material, mat => mat.MapFrom(m => m.CodigoMaterial))
                  .ForMember(x => x.NomChofer, mat => mat.MapFrom(m => m.NombreChofer))
                  .ForMember(x => x.NomTransportista, mat => mat.MapFrom(m => m.NombreTransportista))
                  .ForMember(x => x.NumDocu, mat => mat.MapFrom(m => m.NroDocumentoChofer))
                  .ForMember(x => x.Patente, mat => mat.MapFrom(m => m.PatenteCamion))
                  .ForMember(x => x.PatenteRemolque, mat => mat.MapFrom(m => m.PatenteAcoplado))
                  .ForMember(x => x.TipoDocu, mat => mat.MapFrom(m => m.TipoDocumentoChofer));

            Mapper.CreateMap<EgresoSinFleteFazones, EgresoSinFleteFasonesTransmisionASap>()
                  .ForMember(x => x.Almacen, mat => mat.MapFrom(m => m.Almacen))
                  .ForMember(x => x.CuitClienteDestinatario, mat => mat.MapFrom(m => m.CUITCliente))
                  .ForMember(x => x.CuitTransportista, mat => mat.MapFrom(m => m.CUITD))
                  .ForMember(x => x.Cantidad, mat => mat.MapFrom(m => m.Cantidad))
                  .ForMember(x => x.Centro, mat => mat.MapFrom(m => m.Centro))
                  .ForMember(x => x.FechaCon, mat => mat.MapFrom(m => m.FechaCon))
                  .ForMember(x => x.FechaDoc, mat => mat.MapFrom(m => m.FechaDoc))
                  .ForMember(x => x.CodigoMaterial, mat => mat.MapFrom(m => m.Material))
                  .ForMember(x => x.NombreChofer, mat => mat.MapFrom(m => m.NomChofer))
                  .ForMember(x => x.NombreTransportista, mat => mat.MapFrom(m => m.NomTransportista))
                  .ForMember(x => x.NroDocumentoChofer, mat => mat.MapFrom(m => m.NumDocu))
                  .ForMember(x => x.PatenteCamion, mat => mat.MapFrom(m => m.Patente))
                  .ForMember(x => x.PatenteAcoplado, mat => mat.MapFrom(m => m.PatenteRemolque))
                  .ForMember(x => x.TipoDocumentoChofer, mat => mat.MapFrom(m => m.TipoDocu));
        }
    }
}