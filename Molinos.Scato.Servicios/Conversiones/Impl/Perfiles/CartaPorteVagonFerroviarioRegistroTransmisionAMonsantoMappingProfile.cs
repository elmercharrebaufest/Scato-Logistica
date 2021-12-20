using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CartaPorteVagonFerroviarioRegistroTransmisionAMonsantoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CartaPorteVagonFerroviarioRegistroTransmisionAMonsantoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CartaPorteVagonFerroviarioRegistroTransmisionAMonsanto, CartaPorteVagonFerroviarioRegistro>()
                .ForMember(x => x.cantidadVagones, mat => mat.MapFrom(m => m.CantidadVagones))
                .ForMember(x => x.cantidadVagonesSpecified, mat => mat.MapFrom(m => m.CantidadVagones > 0))
                .ForMember(x => x.numeroCartaPorte, mat => mat.MapFrom(m => m.NumeroCartaPorte))
                .ForMember(x => x.numeroCartaPorteSpecified, mat => mat.MapFrom(m => m.NumeroCartaPorte > 0))
                .ForMember(x => x.destinoGranos, mat => mat.MapFrom(m => new DestinoGranos { codigoLocalidadDestino = m.CodigoLocalidadDestino,numeroPlantaDestino = m.NumeroPlantaDestino}))
                .ForMember(x => x.granosTransportados, mat => mat.MapFrom(m => new GranosTransportados{codigoBiotecnologiaDeclarada = m.CodigoBiotecnologiaDeclarada,codigoEspecie = m.CodigoEspecie,codigoLocalidadProcedencia = m.CodigoLocalidadProcedencia,codigoLocalidadProcedenciaSpecified = m.CodigoLocalidadProcedencia > 0,establecimiento = m.Establecimiento}))
                .ForMember(x => x.intervinientes, mat => mat.MapFrom(m => new IntervinientesRegistro { destinatario = !string.IsNullOrEmpty(m.DestinatarioCuit) ? new DocumentoInterviniente { cuit = m.DestinatarioCuit } : null, destino = !string.IsNullOrEmpty(m.DestinoCuit) ? new DocumentoInterviniente { cuit = m.DestinoCuit } : null, remitenteComercial = !string.IsNullOrEmpty(m.RemitenteComercialCuit) ? new Interviniente { cuit = m.RemitenteComercialCuit, razonSocial = m.RemitenteComercial } : null, titular = !string.IsNullOrEmpty(m.TitularCuit) ? new IntervinienteRequerido { cuit = m.TitularCuit, razonSocial = m.Titular } : null }));
            Mapper.CreateMap<CartaPorteVagonFerroviarioRegistro, CartaPorteVagonFerroviarioRegistroTransmisionAMonsanto>()
                .ForMember(x => x.CantidadVagones, mat => mat.MapFrom(m => m.cantidadVagones))
                .ForMember(x => x.CodigoBiotecnologiaDeclarada, mat => mat.MapFrom(m => m.granosTransportados.codigoBiotecnologiaDeclarada))
                .ForMember(x => x.CodigoEspecie, mat => mat.MapFrom(m => m.granosTransportados.codigoEspecie))
                .ForMember(x => x.CodigoLocalidadDestino, mat => mat.MapFrom(m => m.destinoGranos.codigoLocalidadDestino))
                .ForMember(x => x.CodigoLocalidadProcedencia, mat => mat.MapFrom(m => m.granosTransportados.codigoLocalidadProcedencia))
                .ForMember(x => x.DestinatarioCuit, mat => mat.MapFrom(m => m.intervinientes.destinatario.cuit))
                .ForMember(x => x.DestinoCuit, mat => mat.MapFrom(m => m.intervinientes.destino.cuit))
                .ForMember(x => x.Establecimiento, mat => mat.MapFrom(m => m.granosTransportados.establecimiento))
                .ForMember(x => x.NumeroCartaPorte, mat => mat.MapFrom(m => m.numeroCartaPorte))
                .ForMember(x => x.NumeroPlantaDestino, mat => mat.MapFrom(m => m.destinoGranos.numeroPlantaDestino))
                .ForMember(x => x.RemitenteComercial, mat => mat.MapFrom(m => m.intervinientes.remitenteComercial.razonSocial))
                .ForMember(x => x.RemitenteComercialCuit, mat => mat.MapFrom(m => m.intervinientes.remitenteComercial.cuit))
                .ForMember(x => x.Titular, mat => mat.MapFrom(m => m.intervinientes.titular.razonSocial))
                .ForMember(x => x.TitularCuit, mat => mat.MapFrom(m => m.intervinientes.titular.cuit));
        }
    }
}