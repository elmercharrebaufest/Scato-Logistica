using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaElementoGraficoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaElementoGraficoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaElementoGrafico, ModuloDeCargaElementoGraficoDto>();
            Mapper.CreateMap<ModuloDeCargaElementoGraficoDto, ModuloDeCargaElementoGrafico>();
        }
    }
}