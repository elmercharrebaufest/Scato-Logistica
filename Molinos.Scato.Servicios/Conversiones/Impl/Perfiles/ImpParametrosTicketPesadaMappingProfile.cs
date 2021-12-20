using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpParametrosTicketPesadaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpParametrosTicketPesadaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpTicketPesada, ImpTicketPesadaDto>();
            Mapper.CreateMap<ImpTicketPesadaDto, ImpTicketPesada>();
        }
    }
}
