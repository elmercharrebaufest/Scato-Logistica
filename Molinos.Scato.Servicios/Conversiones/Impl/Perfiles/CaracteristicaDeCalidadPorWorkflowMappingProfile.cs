using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CaracteristicaDeCalidadPorWorkflowMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CaracteristicaDeCalidadPorWorkflowMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CaracteristicaDeCalidadPorWorkflow, CaracteristicaDeCalidadPorWorkflowDto>()
                .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.CaracteristicaDeCalidad.MaterialPorCentro.Material.Id))
                .ForMember(t => t.MaterialDescripcion, f => f.MapFrom(r => r.CaracteristicaDeCalidad.MaterialPorCentro.Material.Descripcion))
                .ForMember(t => t.CaracteristicaDeCalidadId, f => f.MapFrom(r => r.CaracteristicaDeCalidad.Id))
                .ForMember(t => t.CaracteristicaDeCalidadDesc, f => f.MapFrom(r => r.CaracteristicaDeCalidad.DescripcionCorta))
                .ForMember(t => t.WorkflowDesc, f => f.MapFrom(r => r.Workflow.Descripcion));
            Mapper.CreateMap<CaracteristicaDeCalidadPorWorkflowDto, CaracteristicaDeCalidadPorWorkflow>();
        }
    }
}