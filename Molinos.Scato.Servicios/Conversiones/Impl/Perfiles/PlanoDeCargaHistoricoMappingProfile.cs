using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PlanoDeCargaHistoricoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PlanoDeCargaHistoricoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<PlanoDeCargaHistorico, PlanoDeCargaHistoricoDto>()
                .ForMember(x => x.PlanoDeCargaBodegasHistorico,
                x => x.MapFrom(y => y.PlanoDeCargaBodegaHistorico));
            Mapper.CreateMap<PlanoDeCargaHistoricoDto, PlanoDeCargaHistorico>();
        }
    }
}
