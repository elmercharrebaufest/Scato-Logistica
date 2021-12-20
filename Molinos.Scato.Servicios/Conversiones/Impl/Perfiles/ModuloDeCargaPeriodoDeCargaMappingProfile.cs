using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaPeriodoDeCargaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaPeriodoDeCargaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaPeriodoDeCarga, ModuloDeCargaPeriodoDeCargaDto>();
            Mapper.CreateMap<ModuloDeCargaPeriodoDeCargaDto, ModuloDeCargaPeriodoDeCarga>();
        }
    }
}