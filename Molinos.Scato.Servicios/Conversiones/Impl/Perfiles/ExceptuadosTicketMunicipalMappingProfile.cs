using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ExceptuadosTicketMunicipalMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ExceptuadosTicketMunicipalMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<ExceptuadosTicketMunicipal, ExceptuadosTicketMunicipalDto>();
            Mapper.CreateMap<ExceptuadosTicketMunicipalDto, ExceptuadosTicketMunicipal>();
        }
    }
}
