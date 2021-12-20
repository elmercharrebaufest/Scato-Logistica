using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaLineasDeEmbarqueMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaLineasDeEmbarqueMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaLineasDeEmbarque, ModuloDeCargaLineasDeEmbarqueDto>();
            Mapper.CreateMap<ModuloDeCargaLineasDeEmbarqueDto, ModuloDeCargaLineasDeEmbarque>();
        }
    }
}