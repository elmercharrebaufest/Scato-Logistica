using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpIdentificacionMicromuestraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpIdentificacionMicromuestraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpIdentificacionMicromuestra, ImpIdentificacionMicromuestraDto>();
            Mapper.CreateMap<ImpIdentificacionMicromuestraDto, ImpIdentificacionMicromuestra>();
        }
    }
}
