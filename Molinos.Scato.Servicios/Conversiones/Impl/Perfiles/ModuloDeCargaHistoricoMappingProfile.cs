using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaHistoricoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaHistoricoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaHistorico, ModuloDeCargaHistoricoDto>()
                .ForMember(x => x.ModuloDeCargaElementoGraficoHistorico,
                x => x.MapFrom(y => y.ModuloDeCargaElementoGraficoHistorico))
                .ForMember(x => x.ModuloDeCargaManosDeEmbarqueHistorico,
                x => x.MapFrom(y => y.ModuloDeCargaManosDeEmbarqueHistorico))
                .ForMember(x => x.ModuloDeCargaTabiquesDeEmbarqueHistorico,
                x => x.MapFrom(y => y.ModuloDeCargaTabiquesDeEmbarqueHistorico))
                 .ForMember(x => x.ModuloDeCargaHabilitacionDeTanquesHistorico,
                x => x.MapFrom(y => y.ModuloDeCargaHabilitacionDeTanquesHistorico))
                  .ForMember(x => x.ModuloDeCargaLineasDeEmbarqueHistorico,
                x => x.MapFrom(y => y.ModuloDeCargaLineasDeEmbarqueHistorico));
            Mapper.CreateMap<ModuloDeCargaHistoricoDto, ModuloDeCargaHistorico>();
        }
    }
}