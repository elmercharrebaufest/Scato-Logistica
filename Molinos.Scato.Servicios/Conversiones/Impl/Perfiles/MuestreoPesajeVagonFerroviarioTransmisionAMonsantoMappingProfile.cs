using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MuestreoPesajeVagonFerroviarioTransmisionAMonsantoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MuestreoPesajeVagonFerroviarioTransmisionAMonsantoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MuestreoPesajeVagonFerroviario, MuestreoPesajeVagonFerroviarioTransmisionAMonsanto>()
                .ForMember(x => x.CuitLaboratorio, e => e.MapFrom(m => m.cuitLaboratorio))
                .ForMember(x => x.FechaDeDescarga, e => e.MapFrom(m => m.fechaHoraConfirmacionDefinitiva))
                .ForMember(x => x.DatosPorVagon, e => e.MapFrom(m => new List<Vagon>()))
                .ForMember(x => x.NroCartaPorte, e => e.MapFrom(m => m.nroCartaPorte))
                ;
            Mapper.CreateMap<MuestreoPesajeVagonFerroviarioTransmisionAMonsanto, MuestreoPesajeVagonFerroviario>()
                .ForMember(x => x.cuitLaboratorio, e => e.MapFrom(m => m.CuitLaboratorio))
                .ForMember(x => x.fechaHoraConfirmacionDefinitiva, e => e.MapFrom(m => m.FechaDeDescarga))
                .ForMember(x => x.fechaHoraConfirmacionDefinitivaSpecified, e => e.MapFrom(m => m.FechaDeDescarga != null))
                .ForMember(x => x.nroCartaPorte, e => e.MapFrom(m => m.NroCartaPorte))
                .ForMember(x => x.datosPorVagon, e => e.MapFrom(m => m.DatosPorVagon.Select(x => new vagon{idMuestra = x.IdMuestra,kilosNetosSecos = x.KilosNetosSecos,kilosNetosSecosSpecified = x.KilosNetosSecos > 0,numero = x.Numero}).ToArray()))
                 ;
        }
    }
}