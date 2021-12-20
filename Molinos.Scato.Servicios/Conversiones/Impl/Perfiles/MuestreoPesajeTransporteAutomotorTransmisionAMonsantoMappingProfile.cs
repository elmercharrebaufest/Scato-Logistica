using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MuestreoPesajeTransporteAutomotorTransmisionAMonsantoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MuestreoPesajeTransporteAutomotorTransmisionAMonsantoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MuestreoPesajeTransporteAutomotor, MuestreoPesajeTransporteAutomotorTransmisionAMonsanto>()
                .ForMember(x => x.CuitLaboratorio, e => e.MapFrom(m => m.cuitLaboratorio))
                .ForMember(x => x.IdMuestra, e => e.MapFrom(m => m.idMuestra))
                .ForMember(x => x.FechaDeDescarga, e => e.MapFrom(m => m.fechaHoraConfirmacionDefinitiva))
                .ForMember(x => x.KilosNetosSecos, e => e.MapFrom(m => m.kilosNetosSecos))
                .ForMember(x => x.NroCartaPorte, e => e.MapFrom(m => m.nroCartaPorte))
                ;
            Mapper.CreateMap<MuestreoPesajeTransporteAutomotorTransmisionAMonsanto, MuestreoPesajeTransporteAutomotor>()
                .ForMember(x => x.cuitLaboratorio, e => e.MapFrom(m => m.CuitLaboratorio))
                .ForMember(x => x.idMuestra, e => e.MapFrom(m => m.IdMuestra))
                .ForMember(x => x.fechaHoraConfirmacionDefinitiva, e => e.MapFrom(m => m.FechaDeDescarga))
                .ForMember(x => x.fechaHoraConfirmacionDefinitivaSpecified, e => e.MapFrom(m => m.FechaDeDescarga != null))
                .ForMember(x => x.kilosNetosSecos, e => e.MapFrom(m => m.KilosNetosSecos))
                .ForMember(x => x.kilosNetosSecosSpecified, e => e.MapFrom(m => m.KilosNetosSecos > 0))
                .ForMember(x => x.nroCartaPorte, e => e.MapFrom(m => m.NroCartaPorte))
                 ;
        }
    }
}