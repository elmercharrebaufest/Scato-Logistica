using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MotivoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MotivoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Motivo, MotivoDto>();
            Mapper.CreateMap<MotivoDto, Motivo>();
        }
    }
}