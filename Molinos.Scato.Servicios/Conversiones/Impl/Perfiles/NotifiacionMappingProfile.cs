using System.Globalization;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class NotifiacionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "NotifiacionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Notificacion, NotificacionDto>()
                .ForMember(t => t.HoraVista, f => f.MapFrom(r => r.Hora.ToString(CultureInfo.InvariantCulture)));
            Mapper.CreateMap<NotificacionDto, Notificacion>();
        }
    }
}
