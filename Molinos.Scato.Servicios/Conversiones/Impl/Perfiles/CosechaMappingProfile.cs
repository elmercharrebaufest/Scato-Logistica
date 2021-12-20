using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CosechaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CosechaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Cosecha, CosechaDto>();
            Mapper.CreateMap<CosechaDto, Cosecha>();
        }
    }
}