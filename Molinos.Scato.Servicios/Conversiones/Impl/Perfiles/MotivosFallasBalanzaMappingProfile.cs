using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MotivosFallasBalanzaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MotivosFallasBalanzaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MotivosFallasBalanza, MotivosFallasBalanzaDto>();
            Mapper.CreateMap<MotivosFallasBalanzaDto, MotivosFallasBalanza>();
        }
    }
}