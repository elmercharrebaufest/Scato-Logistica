using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaPlanillaDeTurnosTurnosMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaPlanillaDeTurnosTurnosMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaPlanillaDeTurnosTurnos, ModuloDeCargaPlanillaDeTurnosTurnosDto>()
                .ForMember(x => x.ModuloDeCargaPlanillaDeTurnosTurnosDetalles,
                x => x.MapFrom(y => y.ModuloDeCargaPlanillaDeTurnosTurnosDetalles))
                .ForMember(x => x.ModuloDeCargaPlanillaDeTurnosTurnosCortes,
                x => x.MapFrom(y => y.ModuloDeCargaPlanillaDeTurnosTurnosCortes));
            Mapper.CreateMap<ModuloDeCargaPlanillaDeTurnosTurnosDto, ModuloDeCargaPlanillaDeTurnosTurnos>();
        }
    }
}