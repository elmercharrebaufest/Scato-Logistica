using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ControlDeBalanzaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ControlDeBalanzaMappingProfile"; }
        }
        protected override void Configure()
        {

            Mapper.CreateMap<ControlDeBalanza, ControlDeBalanzaDto>()
                  .ForMember(x => x.InstanciaWorkflow, b => b.MapFrom(balanza => balanza.Recorrido.InstanciaWorkflow))
                  .ForMember(x => x.ControlDeBalanzaId, b => b.MapFrom(balanza => balanza.Id))
                  .ForMember(x => x.RecorridoId, b => b.MapFrom(balanza => balanza.Recorrido.Id))
                  .ForMember(x => x.Material, b => b.MapFrom(balanza => balanza.Recorrido.Material.Descripcion))
                  .ForMember(x => x.TipoComercial, b => b.MapFrom(balanza => balanza.Recorrido.TipoComercial.Descripcion))
                  .ForMember(x => x.Patente, b => b.MapFrom(balanza => balanza.Recorrido.Patente))
                  .ForMember(x => x.NumeroDocumentoIngreso, b => b.MapFrom(balanza => balanza.Recorrido.NumeroDocumentoIngreso));
            Mapper.CreateMap<ControlDeBalanzaDto, ControlDeBalanza>();
        }
    }
}
