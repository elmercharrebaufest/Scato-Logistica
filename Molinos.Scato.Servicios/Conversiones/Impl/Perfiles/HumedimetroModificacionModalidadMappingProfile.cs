using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class HumedimetroModificacionModalidadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "HumedimetroModificacionModalidadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<HumedimetroModificacionModalidad, HumedimetroModificacionModalidadDto>();
            Mapper.CreateMap<HumedimetroModificacionModalidadDto, HumedimetroModificacionModalidad>();
        }
    }
}
