using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EstadoConexionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EstadoConexionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<EstadoConexion, EstadoConexionDto>()
                .ForMember(x => x.PuestoDeTrabajoId, mat => mat.MapFrom(m => m.PuestoDeTrabajo.Id));
            Mapper.CreateMap<EstadoConexionDto, EstadoConexion>();
        }
    }
}