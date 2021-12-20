using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaBalanzasMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaBalanzasMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaBalanzas, ModuloDeCargaBalanzasDto>();
            Mapper.CreateMap<ModuloDeCargaBalanzasDto, ModuloDeCargaBalanzas>();
           
        }
    }
}