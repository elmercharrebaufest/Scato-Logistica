using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MotivoReasignacionDeTarjetaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MotivoReasignacionDeTarjetaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MotivoReasignacionDeTarjeta, MotivoReasignacionDeTarjetaDto>();
            Mapper.CreateMap<MotivoReasignacionDeTarjetaDto, MotivoReasignacionDeTarjeta>();
        }
    }
}