using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AutomatismoTipoLlamadoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AutomatismoTipoLlamadoMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<AutomatismoTipoLlamado, AutomatismoTipoLlamadoDto>();
            Mapper.CreateMap<AutomatismoTipoLlamadoDto, AutomatismoTipoLlamado>();
        }
    }
}
