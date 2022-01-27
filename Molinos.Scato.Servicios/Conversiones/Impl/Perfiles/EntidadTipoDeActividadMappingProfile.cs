using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EntidadTipoDeActividadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EntidadTipoDeActividadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<EntidadTipoDeActividad, EntidadTipoDeActividadDto>()
                .ForMember(t => t.EntidadId, f => f.MapFrom(r => r.Entidad.Id))
                .ForMember(t => t.TipoDeActividadId, f => f.MapFrom(r => r.TipoDeActividad.Id));
            Mapper.CreateMap<EntidadTipoDeActividadDto, EntidadTipoDeActividad>();
        }
    }
}
