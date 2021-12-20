using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class SalidaDeOrigenEnRedespachosTransmisionASapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "SalidaDeOrigenEnRedespachosTransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<SalidaDeOrigenEnRedespachosTransmisionASap, Mov975>()
                    .ForMember(x => x.FechaContab, mat => mat.MapFrom(m => m.FechaContab))
                    .ForMember(x => x.FechaDoc, mat => mat.MapFrom(m => m.FechaDoc))
                    .ForMember(x => x.Kilometros, mat => mat.MapFrom(m => m.Kilometros.HasValue ? m.Kilometros : 0))
                    .ForMember(x => x.KilometrosSpecified, mat => mat.MapFrom(m => m.Kilometros.HasValue))
                    ;
            Mapper.CreateMap<Mov975, SalidaDeOrigenEnRedespachosTransmisionASap>()
                    .ForMember(x => x.FechaContab, mat => mat.MapFrom(m => m.FechaContab))
                    .ForMember(x => x.FechaDoc, mat => mat.MapFrom(m => m.FechaDoc))
                    .ForMember(x => x.Kilometros, mat => mat.MapFrom(m => m.KilometrosSpecified ? m.Kilometros : (decimal?)null));
        }
    }
}