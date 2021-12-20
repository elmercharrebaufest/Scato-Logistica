using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpConstanciaDeEntregaLaserMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpConstanciaDeEntregaLaserMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpConstanciaDeEntregaLaser, ImpConstanciaDeEntregaLaserDto>();
            Mapper.CreateMap<ImpConstanciaDeEntregaLaserDto, ImpConstanciaDeEntregaLaser>();
        }
    }
}
