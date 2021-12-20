using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaPlanillaDeTurnosTurnosDetallesMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaPlanillaDeTurnosTurnosDetallesMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaPlanillaDeTurnosTurnosDetalles, ModuloDeCargaPlanillaDeTurnosTurnosDetallesDto>();
            Mapper.CreateMap<ModuloDeCargaPlanillaDeTurnosTurnosDetallesDto, ModuloDeCargaPlanillaDeTurnosTurnosDetalles>();
        }
    }
}