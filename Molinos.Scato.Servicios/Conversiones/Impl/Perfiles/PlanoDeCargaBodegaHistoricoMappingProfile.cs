using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PlanoDeCargaBodegaHistoricoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PlanoDeCargaBodegaHistoricoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<PlanoDeCargaBodegaHistorico, PlanoDeCargaBodegaHistoricoDto>();
            Mapper.CreateMap<PlanoDeCargaBodegaHistoricoDto, PlanoDeCargaBodegaHistorico>();
        }
    }
}
