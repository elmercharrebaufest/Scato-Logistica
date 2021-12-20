using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaManosDeEmbarqueMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaManosDeEmbarqueMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaManosDeEmbarque, ModuloDeCargaManosDeEmbarqueDto>()
                .ForMember(x => x.ModuloDeCargaManosDeEmbarqueDetalle,
                x => x.MapFrom(y => y.ModuloDeCargaManosDeEmbarqueDetalle));
            Mapper.CreateMap<ModuloDeCargaManosDeEmbarqueDto, ModuloDeCargaManosDeEmbarque>();
        }
    }
}