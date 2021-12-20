using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpIdentificacionEnvioLoteACamaraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpIdentificacionEnvioLoteACamaraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpIdentificacionEnvioLoteACamara, ImpIdentificacionEnvioLoteACamaraDto>();
            Mapper.CreateMap<ImpIdentificacionEnvioLoteACamaraDto, ImpIdentificacionEnvioLoteACamara>();
        }
    }
}
