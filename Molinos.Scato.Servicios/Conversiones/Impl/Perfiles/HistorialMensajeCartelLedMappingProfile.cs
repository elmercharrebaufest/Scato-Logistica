using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class HistorialMensajeCartelLedMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "HistorialMensajeCartelLedMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<HistorialMensajeCartelLed, HistorialMensajeCartelLedDto>()
                  .ForMember(t => t.RecorridoId, f => f.MapFrom(r => r.Recorrido.Id))
                  .ForMember(t => t.CalleNombre, f => f.MapFrom(r => r.Calle.Nombre))
                  .ForMember(t => t.CalleId, f => f.MapFrom(r => r.Calle.Id))
                  .ForMember(t => t.CalleColorFondo, f => f.MapFrom(r => r.Calle.Material.ColorFondo))
                  .ForMember(t => t.CalleColorTexto, f => f.MapFrom(r => r.Calle.Material.ColorTexto));
            Mapper.CreateMap<HistorialMensajeCartelLedDto, HistorialMensajeCartelLed>();
        }
    }
}