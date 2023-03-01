using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DomicilioMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DomicilioMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Domicilio, DomicilioDto>();
            Mapper.CreateMap<DomicilioDto, Domicilio>();
        }
    }
}