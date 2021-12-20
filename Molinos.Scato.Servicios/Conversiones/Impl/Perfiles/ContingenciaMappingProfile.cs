using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class BalanzaModificacionModalidadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "BalanzaModificacionModalidadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<BalanzaModificacionModalidad, BalanzaModificacionModalidadDto>();
            Mapper.CreateMap<BalanzaModificacionModalidadDto, BalanzaModificacionModalidad>();
        }
    }
}
