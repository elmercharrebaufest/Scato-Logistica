using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpParametrosTicketPesadaBodegaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpParametrosTicketPesadaBodegaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpTicketPesadaBodega, ImpTicketPesadaBodegaDto>();
            Mapper.CreateMap<ImpTicketPesadaBodegaDto, ImpTicketPesadaBodega>();
        }
    }
}
