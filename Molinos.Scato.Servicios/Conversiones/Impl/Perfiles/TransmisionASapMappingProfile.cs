using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TransmisionASapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TransmisionASap, TransmisionASapDto>();
            Mapper.CreateMap<TransmisionASapDto, TransmisionASap>();
        }
    }
}