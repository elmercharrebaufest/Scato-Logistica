using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PlanoDeCargaBodegaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PlanoDeCargaBodegaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<PlanoDeCargaBodega, PlanoDeCargaBodegaDto>();
            Mapper.CreateMap<PlanoDeCargaBodegaDto, PlanoDeCargaBodega>();
        }
    }
}