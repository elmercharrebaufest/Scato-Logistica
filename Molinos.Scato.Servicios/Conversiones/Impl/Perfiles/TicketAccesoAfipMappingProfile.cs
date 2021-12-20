using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TicketAccesoAfipMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TicketAccesoAfipMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TicketAccesoAfip, TicketAccesoAfipDto>();
            Mapper.CreateMap<TicketAccesoAfipDto, TicketAccesoAfip>();
        }
    }
}