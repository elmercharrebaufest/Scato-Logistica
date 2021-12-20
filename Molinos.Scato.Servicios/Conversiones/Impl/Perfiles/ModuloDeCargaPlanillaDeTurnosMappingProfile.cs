using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaPlanillaDeTurnosMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaPlanillaDeTurnosMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaPlanillaDeTurnos, ModuloDeCargaPlanillaDeTurnosDto>()
                .ForMember(x => x.ModuloDeCargaPlanillaDeTurnosTurnos,
                x => x.MapFrom(y => y.ModuloDeCargaPlanillaDeTurnosTurnos));
            Mapper.CreateMap<ModuloDeCargaPlanillaDeTurnosDto, ModuloDeCargaPlanillaDeTurnos>();
        }
    }
}