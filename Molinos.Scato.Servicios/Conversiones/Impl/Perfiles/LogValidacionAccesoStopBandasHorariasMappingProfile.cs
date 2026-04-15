using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LogValidacionAccesoStopBandasHorariasMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LogValidacionAccesoStopBandasHorariasMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<LogValidacionAccesoStopBandasHorarias, LogValidacionAccesoStopBandasHorariasDto>();
            Mapper.CreateMap<LogValidacionAccesoStopBandasHorariasDto, LogValidacionAccesoStopBandasHorarias>();
        }

    }
}