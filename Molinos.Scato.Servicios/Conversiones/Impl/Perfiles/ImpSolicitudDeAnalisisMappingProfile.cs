using System.Linq;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpSolicitudDeAnalisisMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpSolicitudDeAnalisisMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpSolicitudDeAnalisis, ImpSolicitudDeAnalisisDto>()
                .ForMember(x => x.CaracteristicaDeCalidad, mat => mat.MapFrom(m => m.CaracteristicaDeCalidad.Split(',')));

            Mapper.CreateMap<ImpSolicitudDeAnalisisDto, ImpSolicitudDeAnalisis>()
                .ForMember(x => x.CaracteristicaDeCalidad, mat => mat.MapFrom(m => m.CaracteristicaDeCalidad.Aggregate((a, b) => a + ',' + b)));

        }
    }
}
