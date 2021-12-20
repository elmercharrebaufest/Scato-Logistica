using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class HistoricoInhabilitacionChoferMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "HistoricoInhabilitacionChoferMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<HistoricoInhabilitacionChofer, HistoricoInhabilitacionChoferDto>();
            Mapper.CreateMap<HistoricoInhabilitacionChoferDto, HistoricoInhabilitacionChofer>();
        }
    }
}