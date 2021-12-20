using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LectorMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LectorMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Lector, LectorDto>();
            Mapper.CreateMap<LectorDto, Lector>();
        }
    }
}