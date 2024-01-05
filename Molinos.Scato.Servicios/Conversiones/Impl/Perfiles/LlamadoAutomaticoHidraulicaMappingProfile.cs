using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LlamadoAutomaticoHidraulicaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LlamadoAutomaticoHidraulica"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LlamadoAutomaticoHidraulica, LlamadoAutomaticoHidraulicaDto>()
                .ForMember(t => t.HidraulicaId, f => f.MapFrom(r => r.Hidraulica.Id))
                .ForMember(t => t.HidraulicaNombre, f => f.MapFrom(r => r.Hidraulica.Nombre))
                .ForMember(t => t.EsEscalable, f => f.MapFrom(r => r.Hidraulica.EsEscalable))
                .ForMember(t => t.ActivoAutomatico, f => f.MapFrom(r => r.Hidraulica.ActivoAutomatico))
                .ForMember(t => t.CentroId, f => f.MapFrom(r => r.Hidraulica.Centro.Id));
            Mapper.CreateMap<LlamadoAutomaticoHidraulicaDto, LlamadoAutomaticoHidraulica>();
        }
    }
}