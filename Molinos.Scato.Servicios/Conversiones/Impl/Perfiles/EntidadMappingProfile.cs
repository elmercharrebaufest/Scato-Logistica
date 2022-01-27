using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EntidadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EntidadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Entidad, EntidadDto>();
            Mapper.CreateMap<EntidadDto, Entidad>();
        }
    }
}
