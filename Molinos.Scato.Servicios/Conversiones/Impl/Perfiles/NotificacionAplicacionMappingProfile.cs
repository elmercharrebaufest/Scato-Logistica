using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class NotificacionAplicacionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "NotificacionAplicacionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<NotificacionAplicacion, NotificacionAplicacionDto>();
            Mapper.CreateMap<NotificacionAplicacionDto, NotificacionAplicacion>();
        }
    }
}