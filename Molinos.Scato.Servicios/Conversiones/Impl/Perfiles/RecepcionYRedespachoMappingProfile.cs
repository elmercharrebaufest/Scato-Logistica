using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RecepcionYRedespachoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RecepcionYRedespachoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<RecepcionYRedespacho, ZMPES0020>()
                .ForMember(t => t.CARACTERISTICA, f => f.MapFrom(r => r.Caracteristica))
                .ForMember(t => t.DESCKILOS, f => f.MapFrom(r => r.Desckilos))
                .ForMember(t => t.DESCPORC, f => f.MapFrom(r => r.Descporc))
                .ForMember(t => t.ENTRADA_O_SALIDA, f => f.MapFrom(r => r.EntradaOSalida))
                .ForMember(t => t.NUMCARPOR, f => f.MapFrom(r => r.NumCarPor))
                .ForMember(t => t.SECUENCIA, f => f.MapFrom(r => r.Secuencia))
                .ForMember(t => t.TIPO_MUEST, f => f.MapFrom(r => r.TipoMuest))
                .ForMember(t => t.RESULTADO, f => f.MapFrom(r => r.Resultado));
            Mapper.CreateMap<ZMPES0020, RecepcionYRedespacho>()
                .ForMember(t => t.Caracteristica, f => f.MapFrom(r => r.CARACTERISTICA))
                .ForMember(t => t.Desckilos, f => f.MapFrom(r => r.DESCKILOS))
                .ForMember(t => t.Descporc, f => f.MapFrom(r => r.DESCPORC))
                .ForMember(t => t.EntradaOSalida, f => f.MapFrom(r => r.ENTRADA_O_SALIDA))
                .ForMember(t => t.NumCarPor, f => f.MapFrom(r => r.NUMCARPOR))
                .ForMember(t => t.Secuencia, f => f.MapFrom(r => r.SECUENCIA))
                .ForMember(t => t.TipoMuest, f => f.MapFrom(r => r.TIPO_MUEST))
                .ForMember(t => t.Resultado, f => f.MapFrom(r => r.RESULTADO));
        }
    }
}