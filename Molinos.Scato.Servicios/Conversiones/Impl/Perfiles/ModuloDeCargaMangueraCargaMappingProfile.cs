using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaMangueraCargaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaMangueraCargaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaMangueraCarga, ModuloDeCargaMangueraCargaDto>();
            Mapper.CreateMap<ModuloDeCargaMangueraCargaDto, ModuloDeCargaMangueraCarga>();
        }
    }
}