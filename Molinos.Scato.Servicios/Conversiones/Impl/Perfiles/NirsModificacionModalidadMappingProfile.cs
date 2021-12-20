using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class NirsModificacionModalidadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "NirsModificacionModalidadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<NirsModificacionModalidad, NirsModificacionModalidadDto>();
            Mapper.CreateMap<NirsModificacionModalidadDto, NirsModificacionModalidad>();
        }
    }
}
