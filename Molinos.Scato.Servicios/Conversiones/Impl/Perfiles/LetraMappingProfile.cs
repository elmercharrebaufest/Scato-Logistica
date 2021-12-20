using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LetraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LetraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Letra, LetraDto>();
            Mapper.CreateMap<LetraDto, Letra>();
        }
    }
}