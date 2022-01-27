using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TipoDeActividadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TipoDeActividadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TipoDeActividad, TipoDeActividadDto>();
            Mapper.CreateMap<TipoDeActividadDto, TipoDeActividad>();
        }
    }
}
