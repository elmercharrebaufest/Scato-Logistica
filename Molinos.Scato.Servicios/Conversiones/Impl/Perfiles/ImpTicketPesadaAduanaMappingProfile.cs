using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpTicketPesadaAduanaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpTicketPesadaAduanaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpTicketPesadaAduana, ImpTicketPesadaAduanaDto>();
            Mapper.CreateMap<ImpTicketPesadaAduanaDto, ImpTicketPesadaAduana>();
        }
    }
}
