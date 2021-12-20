using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System.Linq;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TarjetaSupervisorMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TarjetaSupervisorMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TarjetaSupervisor, TarjetaSupervisorDto>()
                .ForMember(x => x.CentroId, mat => mat.MapFrom(a => a.Centro.Id));
            Mapper.CreateMap<TarjetaSupervisorDto, TarjetaSupervisor>();
        }
    }
}
