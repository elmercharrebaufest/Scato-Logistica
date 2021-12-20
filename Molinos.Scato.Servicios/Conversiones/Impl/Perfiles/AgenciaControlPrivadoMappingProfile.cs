using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AgenciaControlPrivadoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AgenciaControlPrivadoProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AgenciaControlPrivado, AgenciaControlPrivadoDto>();
            Mapper.CreateMap<AgenciaControlPrivadoDto, AgenciaControlPrivado>();
        }
    }
}
