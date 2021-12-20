using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PlanoDeCargaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PlanoDeCargaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<PlanoDeCarga, PlanoDeCargaDto>()
                .ForMember(x => x.PlanoDeCargaBodegas,
                x => x.MapFrom(y => y.PlanoDeCargaBodega))
                .ForMember(x => x.CargasComerciales,
                x => x.MapFrom(y => y.CargaComercial));
            Mapper.CreateMap<PlanoDeCargaDto, PlanoDeCarga>();
        }
    }
}