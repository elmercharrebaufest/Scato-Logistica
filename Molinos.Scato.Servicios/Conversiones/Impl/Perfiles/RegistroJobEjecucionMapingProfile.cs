using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RegistroJobEjecucionMapingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RegistroJobEjecucionMapingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<RegistroJobEjecucion, RegistroJobEjecucionDto>();
            Mapper.CreateMap<RegistroJobEjecucionDto, RegistroJobEjecucion>();
        }
    }
}