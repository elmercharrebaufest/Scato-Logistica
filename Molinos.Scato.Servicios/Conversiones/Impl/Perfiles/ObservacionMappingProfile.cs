using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ObservacionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ObservacionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Observacion, ObservacionDto>();
            Mapper.CreateMap<ObservacionDto, Observacion>();
        }
    }
}
