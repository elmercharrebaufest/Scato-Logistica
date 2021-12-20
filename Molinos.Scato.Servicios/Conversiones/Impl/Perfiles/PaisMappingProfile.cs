using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PaisMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PaisMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Pais, PaisDto>();
            Mapper.CreateMap<PaisDto, Pais>();
        }
    }
}
