using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class FirmaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "FirmaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Firma, FirmaDto>();
            Mapper.CreateMap<FirmaDto, Firma>();
        }
    }
}
