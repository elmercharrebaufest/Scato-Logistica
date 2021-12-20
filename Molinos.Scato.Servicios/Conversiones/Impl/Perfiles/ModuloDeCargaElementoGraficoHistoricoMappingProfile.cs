using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaElementoGraficoHistoricoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaElementoGraficoHistoricoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaElementoGraficoHistorico, ModuloDeCargaElementoGraficoHistoricoDto>();
            Mapper.CreateMap<ModuloDeCargaElementoGraficoHistoricoDto, ModuloDeCargaElementoGraficoHistorico>();
        }
    }
}