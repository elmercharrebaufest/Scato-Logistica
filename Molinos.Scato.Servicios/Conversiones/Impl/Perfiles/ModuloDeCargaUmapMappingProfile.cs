using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaUmapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaUmapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaUmap, ModuloDeCargaUmapDto>();
            Mapper.CreateMap<ModuloDeCargaUmapDto, ModuloDeCargaUmap>();
        }
    }
}