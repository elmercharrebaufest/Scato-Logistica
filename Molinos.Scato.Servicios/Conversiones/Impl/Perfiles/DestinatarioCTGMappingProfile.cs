using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DestinatarioCTGMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DestinatarioCTGMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DestinatarioCTG, DestinatarioCTGDto>();
            Mapper.CreateMap<DestinatarioCTGDto, DestinatarioCTG>();

            Mapper.CreateMap<DestinatarioCTG, ZMPES0510>()
               .ForMember(t => t.CANJEREMITCOM, f => f.MapFrom(r => r.CanjeRemito))
               .ForMember(t => t.CCPP, f => f.MapFrom(r => r.NumeroCCPP))
               .ForMember(t => t.COSECHA, f => f.MapFrom(r => r.Cosecha))
               .ForMember(t => t.FE_HR_CONF, f => f.MapFrom(r => r.FechaConf))
               .ForMember(t => t.CTG, f => f.MapFrom(r => r.CTG))
               .ForMember(t => t.CUIT_CANJEADOR, f => f.MapFrom(r => r.CuitCanjeador))
               .ForMember(t => t.CUIT_DESTINATARI, f => f.MapFrom(r => r.CuitDestinatario))
               .ForMember(t => t.CUIT_DESTINO, f => f.MapFrom(r => r.CuitDestino))
               .ForMember(t => t.ESPECIE, f => f.MapFrom(r => r.Especie))
               .ForMember(t => t.ESTABLECIMIENTO, f => f.MapFrom(r => r.Establecimiento))
               .ForMember(t => t.ESTADO, f => f.MapFrom(r => r.Estado))
               .ForMember(t => t.PESO_NETO_CARGA, f => f.MapFrom(r => r.PesoNetoCarga))
               .ForMember(t => t.SOLICITANTE, f => f.MapFrom(r => r.Solicitante))
               ;

            Mapper.CreateMap<ZMPES0510, DestinatarioCTG>()
                .ForMember(t => t.CanjeRemito, f => f.MapFrom(r => r.CANJEREMITCOM))
                .ForMember(t => t.NumeroCCPP, f => f.MapFrom(r => r.CCPP))
                .ForMember(t => t.Cosecha, f => f.MapFrom(r => r.COSECHA))
                .ForMember(t => t.FechaConf, f => f.MapFrom(r => r.FE_HR_CONF))
                .ForMember(t => t.CTG, f => f.MapFrom(r => r.CTG))
                .ForMember(t => t.CuitCanjeador, f => f.MapFrom(r => r.CUIT_CANJEADOR))
                .ForMember(t => t.CuitDestinatario, f => f.MapFrom(r => r.CUIT_DESTINATARI))
                .ForMember(t => t.CuitDestino, f => f.MapFrom(r => r.CUIT_DESTINO))
                .ForMember(t => t.Especie, f => f.MapFrom(r => r.ESPECIE))
                .ForMember(t => t.Establecimiento, f => f.MapFrom(r => r.ESTABLECIMIENTO))
                .ForMember(t => t.Estado, f => f.MapFrom(r => r.ESTADO))
                .ForMember(t => t.PesoNetoCarga, f => f.MapFrom(r => r.PESO_NETO_CARGA))
                .ForMember(t => t.Solicitante, f => f.MapFrom(r => r.SOLICITANTE))
                ;
        }
    }
}