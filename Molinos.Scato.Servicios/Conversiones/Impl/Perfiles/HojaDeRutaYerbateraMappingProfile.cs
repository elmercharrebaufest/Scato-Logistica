using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class HojaDeRutaYerbateraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "HojaDeRutaYerbateraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<HojaDeRutaYerbatera, HojaDeRutaYerbateraDto>()
                .ForMember(t => t.ProveedorId, f => f.MapFrom(r => r.Proveedor.Id))
                .ForMember(t => t.Proveedor, f => f.MapFrom(r => r.Proveedor.RazonSocial))
                .ForMember(t => t.ProveedorCodigoSap, f => f.MapFrom(r => r.Proveedor.CodigoSap))
                .ForMember(t => t.ProveedorCuil, f => f.MapFrom(r => r.Proveedor.Cuil))
                .ForMember(t => t.Procedencia, f => f.MapFrom(r => r.Procedencia.Descripcion))
                .ForMember(t => t.ProcedenciaId, f => f.MapFrom(r => r.Procedencia.Id))
                .ForMember(t => t.ProcedenciaCodigoSap, f => f.MapFrom(r => r.Procedencia.CodigoAfip))

                .ForMember(t => t.Destinatario, f => f.MapFrom(r => r.Destinatario.Descripcion))
                .ForMember(t => t.DestinatarioId, f => f.MapFrom(r => r.Destinatario.Id))
                .ForMember(t => t.DestinatarioCodigoSap, f => f.MapFrom(r => r.Destinatario.CodigoSap))
                .ForMember(t => t.DestinatarioCuil, f => f.MapFrom(r => r.Destinatario.Cuil))
                .ForMember(t => t.DestinatarioMail, f => f.MapFrom(r => r.Destinatario.EnvioAutomaticoMail && !string.IsNullOrEmpty(r.Destinatario.Mail) ? r.Destinatario.Mail : string.Empty))
                .ForMember(t => t.DestinatarioEnvioMailEnAnalisis, f => f.MapFrom(r => r.Destinatario.Analisis))
                .ForMember(t => t.DestinatarioEnvioMailEnPesada, f => f.MapFrom(r => r.Destinatario.Pesada))

                .ForMember(t => t.CentroDestinoCodigoSap, f => f.MapFrom(r => r.CentroDestino.CodigoSAP))
                .ForMember(t => t.CentroDestino, f => f.MapFrom(r => r.CentroDestino.Descripcion))
                .ForMember(t => t.CentroDestinoId, f => f.MapFrom(r => r.CentroDestino.Id))
                .ForMember(t => t.CentroDestinoDireccion, f => f.MapFrom(r => r.CentroDestino.Direccion))
                .ForMember(t => t.CentroDestinoProvincia, f => f.MapFrom(r => r.CentroDestino.Provincia.Descripcion))
                .ForMember(t => t.CentroDestinoLocalidad, f => f.MapFrom(r => r.CentroDestino.Localidad.Descripcion))
                .ForMember(t => t.CentroDestinoCodigoPostal, f => f.MapFrom(r => r.CentroDestino.CodigoPostal))
                .ForMember(t => t.CentroDestinoCuit, f => f.MapFrom(r => r.CentroDestino.Cuit))

                .ForMember(t => t.Transportista, f => f.MapFrom(r => r.Transportista.RazonSocial))
                .ForMember(t => t.TransportistaId, f => f.MapFrom(r => r.Transportista.Id))
                .ForMember(t => t.TransportistaCUIT, f => f.MapFrom(r => r.Transportista.Cuit))

                .ForMember(t => t.TipoComercial, f => f.MapFrom(r => r.TipoComercial.Descripcion))
                .ForMember(t => t.TipoComercialSentido, f => f.MapFrom(r => r.TipoComercial.Sentido))
                .ForMember(t => t.TipoComercialId, f => f.MapFrom(r => r.TipoComercial.Id))
                .ForMember(t => t.TipoComercialCodigoSap, f => f.MapFrom(r => r.TipoComercial.CodigoSap))

                .ForMember(t => t.Material, f => f.MapFrom(r => r.Material.Descripcion))
                .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                .ForMember(t => t.MaterialPideLote, f => f.MapFrom(r => r.Material.Lote))
                .ForMember(t => t.MaterialPideContrato, f => f.MapFrom(r => r.Material.Contrato))
                .ForMember(t => t.MaterialCodigoSap, f => f.MapFrom(r => r.Material.CodigoSAP))
                .ForMember(t => t.TipoVehiculo, f => f.MapFrom(r => r.Recorrido.TipoVehiculo));
            Mapper.CreateMap<HojaDeRutaYerbateraDto, HojaDeRutaYerbatera>();

        }
    }
}
