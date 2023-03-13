using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MuestraDeInaseMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MuestraDeInaseMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MuestraDeInase, MuestraDeInaseDto>()
                .ForMember(t => t.CentroId, f => f.MapFrom(r => r.Recorrido.Centro.Id))
                .ForMember(t => t.RecorridoId, f => f.MapFrom(r => r.Recorrido.Id))
                .ForMember(t => t.WorkflowInstanceId, f => f.MapFrom(r => r.Recorrido.InstanciaWorkflow))
                .ForMember(t => t.CartaPorte, f => f.MapFrom(r => r.Recorrido.NumeroDocumentoIngreso));
            Mapper.CreateMap<MuestraDeInaseDto, MuestraDeInase>();
        }
    }
}
