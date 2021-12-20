using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ContingenciaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ContingenciaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Contingencia, ContingenciaDto>();
            Mapper.CreateMap<ContingenciaDto, Contingencia>();
        }
    }
}
