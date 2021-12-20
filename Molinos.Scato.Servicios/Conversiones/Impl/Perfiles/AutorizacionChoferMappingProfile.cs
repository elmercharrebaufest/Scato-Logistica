using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class  AutorizacionChoferMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "HistoricoInhabilitacionChoferMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AutorizacionChofer, AutorizacionChoferDto>();
            Mapper.CreateMap<AutorizacionChoferDto, AutorizacionChofer>();
        }
    }
}