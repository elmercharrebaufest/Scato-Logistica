using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ControlDeBalanzaPesadaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ControlDeBalanzaPesadaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ControlDeBalanzaPesada, ControlDeBalanzaPesadaDto>()
                  .ForMember(x => x.BalanzaId, b => b.MapFrom(balanza => balanza.Balanza.Id))
                  .ForMember(x => x.BalanzaNombre, b => b.MapFrom(balanza => balanza.Balanza.Nombre))
                  .ForMember(x => x.Patente, b => b.MapFrom(balanza => balanza.ControlDeBalanza.Recorrido.Patente))
                  .ForMember(x => x.TipoPesada, b => b.MapFrom(balanza => balanza.ControlDeBalanza.TipoPesada))
                  .ForMember(x => x.Observacion, b => b.MapFrom(balanza => balanza.ControlDeBalanza.Observaciones))
                  .ForMember(x => x.NumeroDocumentoIngreso, b => b.MapFrom(balanza => balanza.ControlDeBalanza.Recorrido.NumeroDocumentoIngreso))
                  .ForMember(x => x.TipoDocumentoIngreso, b => b.MapFrom(balanza => balanza.ControlDeBalanza.Recorrido.TipoDocumentoIngreso));
            Mapper.CreateMap<ControlDeBalanzaPesadaDto, ControlDeBalanzaPesada>();
        }
    }
}
