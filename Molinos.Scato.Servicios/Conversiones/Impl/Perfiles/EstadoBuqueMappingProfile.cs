using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EstadoBuqueMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EstadoBuqueMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<EstadoBuque, EstadoBuqueDto>();
            Mapper.CreateMap<EstadoBuqueDto, EstadoBuque>();
        }
    }
}