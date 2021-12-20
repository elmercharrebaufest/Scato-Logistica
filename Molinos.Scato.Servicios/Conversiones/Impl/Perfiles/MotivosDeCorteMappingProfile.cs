using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MotivosDeCorteMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MotivosDeCorteMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MotivosDeCorte, MotivosDeCorteDto>();
            Mapper.CreateMap<MotivosDeCorteDto, MotivosDeCorte>();
        }
    }
}