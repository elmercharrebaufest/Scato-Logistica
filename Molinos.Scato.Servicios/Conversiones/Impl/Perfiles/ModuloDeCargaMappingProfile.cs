using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCarga, ModuloDeCargaDto>()
                .ForMember(x => x.ModuloDeCargaElementoGrafico, 
                x => x.MapFrom(y => y.ModuloDeCargaElementoGrafico))
                .ForMember(x => x.ModuloDeCargaRitmosEmbarque,
                x => x.MapFrom(y => y.ModuloDeCargaRitmosEmbarque))
                .ForMember(x => x.ModuloDeCargaManosDeEmbarque,
                x => x.MapFrom(y => y.ModuloDeCargaManosDeEmbarque))
                .ForMember(x => x.ModuloDeCargaTabiquesDeEmbarque,
                x => x.MapFrom(y => y.ModuloDeCargaTabiquesDeEmbarque))
                .ForMember(x => x.ModuloDeCargaHabilitacionDeTanques,
                x => x.MapFrom(y => y.ModuloDeCargaHabilitacionDeTanques))
                .ForMember(x => x.ModuloDeCargaLineasDeEmbarque,
                x => x.MapFrom(y => y.ModuloDeCargaLineasDeEmbarque))
                .ForMember(x => x.ModuloDeCargaMangueraCarga,
                x => x.MapFrom(y => y.ModuloDeCargaMangueraCarga))
                .ForMember(x => x.ModuloDeCargaPlanillaDeEmbarque,
                x => x.MapFrom(y => y.ModuloDeCargaPlanillaDeEmbarque))
                .ForMember(x => x.ModuloDeCargaPlanillaDeTurnos,
                x => x.MapFrom(y => y.ModuloDeCargaPlanillaDeTurnos))
                .ForMember(x => x.ModuloDeCargaPeriodoDeCarga,
                x => x.MapFrom(y => y.ModuloDeCargaPeriodoDeCarga))
                .ForMember(x => x.ModuloDeCargaUmap,
                x => x.MapFrom(y => y.ModuloDeCargaUmap))
                .ForMember(x => x.ModuloDeCargaBalanzas,
                x => x.MapFrom(y => y.ModuloDeCargaBalanzas));
            Mapper.CreateMap<ModuloDeCargaDto, ModuloDeCarga>();
        }
    }
}