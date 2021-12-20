using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DistribucionDeAlmacenesMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DistribucionDeAlmacenesMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DistribucionDeAlmacenes, DistribucionDeAlmacenesDto>()
                .ForMember(t => t.Centro, f => f.MapFrom(r => r.Recorrido.Centro.Descripcion))
                .ForMember(t => t.InstanceId, f => f.MapFrom(r => r.Recorrido.InstanciaWorkflow))
                .ForMember(t => t.NumeroDeDocumentoDeIngreso, f => f.MapFrom(r => r.Recorrido.NumeroDocumentoIngreso))
                .ForMember(t => t.Patente, f => f.MapFrom(r => r.Recorrido.Patente))
                .ForMember(t => t.Workflow, f => f.MapFrom(r => r.Recorrido.Workflow.Codigo))
                .ForMember(t => t.WorkflowDefinicionId, f => f.MapFrom(r => r.Recorrido.WorkflowDefinicion.Id))
                ;
            Mapper.CreateMap<DistribucionDeAlmacenesDto, DistribucionDeAlmacenes>();
        }
    }
}