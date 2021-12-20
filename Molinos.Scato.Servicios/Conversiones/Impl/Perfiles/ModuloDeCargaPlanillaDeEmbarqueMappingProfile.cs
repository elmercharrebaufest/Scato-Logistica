using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaPlanillaDeEmbarqueMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaPlanillaDeEmbarqueMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaPlanillaDeEmbarque, ModuloDeCargaPlanillaDeEmbarqueDto>();
            Mapper.CreateMap<ModuloDeCargaPlanillaDeEmbarqueDto, ModuloDeCargaPlanillaDeEmbarque>();
        }
    }
}