using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaRitmosEmbarqueMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaRitmosEmbarqueMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaRitmosEmbarque, ModuloDeCargaRitmosEmbarqueDto>();
            Mapper.CreateMap<ModuloDeCargaRitmosEmbarqueDto, ModuloDeCargaRitmosEmbarque>();
        }
    }
}