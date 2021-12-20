using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MaterialPorWorkflowMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MaterialPorWorkflowMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MaterialPorWorkflow, MaterialPorWorkflowDto>()
                  .ForMember(t => t.MaterialDesc, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.MaterialCodigoSap, f => f.MapFrom(r => r.Material.CodigoSAP))
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                  .ForMember(t => t.WorkflowDesc, f => f.MapFrom(r => r.Workflow.Descripcion))
                  .ForMember(t => t.WorkflowId, f => f.MapFrom(r => r.Workflow.Id))
                  .ForMember(t => t.CentroDesc, f => f.MapFrom(r => r.Centro.Descripcion))
                  .ForMember(t => t.RequiereAnexoInase, f => f.MapFrom(r => r.Material.RequiereAnexoInase))
                  .ForMember(t => t.CentroId, f => f.MapFrom(r => r.Centro.Id))
                  .ForMember(t => t.ClienteDesc, f => f.MapFrom(r => r.Cliente.Descripcion))
                  .ForMember(t => t.ClienteId, f => f.MapFrom(r => r.Cliente.Id))
                  .ForMember(t => t.EsCosecha, f => f.MapFrom(r => r.Material.EsCosecha))
                  .ForMember(t => t.VigenciaDesde, f => f.MapFrom(r => r.Material.VigenciaDesde))
                  .ForMember(t => t.VigenciaHasta, f => f.MapFrom(r => r.Material.VigenciaHasta));
            Mapper.CreateMap<MaterialPorWorkflowDto, MaterialPorWorkflow>();

            Mapper.CreateMap<MaterialPorWorkflow, TipoBinDto>()
                  .ForMember(t => t.Id, f => f.MapFrom(r => r.Material.Id))
                  .ForMember(t => t.Descripcion, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.Peso, f => f.MapFrom(r => r.Material.Peso));

            Mapper.CreateMap<MaterialPorWorkflow, CaracteristicaDeCalidadPorWorkflowDto>()
                .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                .ForMember(t => t.MaterialDescripcion, f => f.MapFrom(r => r.Material.Descripcion))
                .ForMember(t => t.WorkflowDesc, f => f.MapFrom(r => r.Workflow.Descripcion))
                .ForMember(t => t.WorkflowTipo, f => f.MapFrom(r => r.Workflow.TipoDeWorkflow.DisplayEnum()))
                .ForMember(t => t.WorkflowId, f => f.MapFrom(r => r.Workflow.Id));
        }
    }
}