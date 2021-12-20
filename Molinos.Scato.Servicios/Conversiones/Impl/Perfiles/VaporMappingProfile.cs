using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class VaporMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VaporMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Vapor, VaporDto>();
            Mapper.CreateMap<VaporDto, Vapor>();
        }
    }
}