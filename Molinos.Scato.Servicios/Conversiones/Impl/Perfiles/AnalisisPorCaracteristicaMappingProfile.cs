using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AnalisisPorCaracteristicaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AnalisisPorCaracteristicaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AnalisisPorCaracteristica, AnalisisPorCaracteristicaDto>()
                .ForMember(x => x.CaracteristicaId, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.Id))
                .ForMember(x => x.CaracteristicaCodigoSap, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.CodigoSAP))
                .ForMember(x => x.NoAceptarSiSeDefineUnValor, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.NoAceptarSiSeDefineUnValor))
                .ForMember(x => x.ToleranciaSinAnalisis, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.ToleranciaSinAnalisis))
                .ForMember(x => x.ToleranciaSinMensaje, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.ToleranciaSinMensaje))
                .ForMember(x => x.CaladoMinimo, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.CaladoMinimo))
                .ForMember(x => x.TipoDeEnsayo, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.Ensayo))
                .ForMember(x => x.TipoDeAnalisis, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.Analisis))
                .ForMember(x => x.Caracteristica, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.CaracteristicaDeCalidadMaestro.Descripcion))
                .ForMember(x => x.EsHumedad, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.EsHumedad))
                .ForMember(x => x.EsMermaVolatil, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.EsMermaVolatil))
                .ForMember(x => x.EsInsectosVivos, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.EsInsectosVivos))
                .ForMember(x => x.AnalisisDeCalidadId, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.AnalisisDeCalidad.Id))
                .ForMember(x => x.EnviaASap, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.EnviaASap));
            Mapper.CreateMap<AnalisisPorCaracteristicaDto, AnalisisPorCaracteristica>();
        }
    }
}
