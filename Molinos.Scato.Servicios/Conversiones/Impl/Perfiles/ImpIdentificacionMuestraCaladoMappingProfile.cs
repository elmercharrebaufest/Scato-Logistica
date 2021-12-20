using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpIdentificacionMuestraCaladoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpIdentificacionMuestraCaladoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpIdentificacionMuestraCalado, ImpIdentificacionMuestraCaladoDto>();
            Mapper.CreateMap<ImpIdentificacionMuestraCaladoDto, ImpIdentificacionMuestraCalado>();
        }
    }
}
