using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LlenadoMilimetroPorTanqueMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LlenadoMilimetroPorTanqueMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LlenadoMilimetroPorTanque, LlenadoMilimetroPorTanqueDto>();
            Mapper.CreateMap<LlenadoMilimetroPorTanqueDto, LlenadoMilimetroPorTanque>();
        }
    }
}