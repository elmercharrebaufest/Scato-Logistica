using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CaladoPorCaracteristicaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CaladoPorCaracteristicaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CaladoPorCaracteristica, CaladoPorCaracteristicaDto>()
                .ForMember(x => x.CaracteristicaId, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.Id))
                .ForMember(x => x.Caracteristica, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.CaracteristicaDeCalidadMaestro.Descripcion))
                .ForMember(x => x.CaracteristicaDescripcionCorta, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.DescripcionCorta))
                .ForMember(x => x.NoAceptarSiSeDefineUnValor, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.NoAceptarSiSeDefineUnValor))
                .ForMember(x => x.CaracteristicaSituacionEnvioACamara, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.SituacionEnvioACamara))
                .ForMember(x => x.CaracteristicaSiSuperaValorCamara, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.SiSuperaValorCamara))
                .ForMember(x => x.TipoDeEnsayo, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.Ensayo))
                .ForMember(x => x.TipoDeAnalisis, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.Analisis))
                .ForMember(x => x.EsModificable, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.EsModificable))
                .ForMember(x => x.EsHumedad, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.EsHumedad))
                .ForMember(x => x.EsMermaVolatil, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.EsMermaVolatil))
                .ForMember(x => x.CaracteristicaCodigoSap, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.CodigoSAP))
                .ForMember(x => x.EnviaASap, cxc => cxc.MapFrom(calPorCaracteristica => calPorCaracteristica.CaracteristicaDeCalidad.EnviaASap));
            Mapper.CreateMap<CaladoPorCaracteristicaDto, CaladoPorCaracteristica>();
        }
    }
}
