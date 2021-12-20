using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AjusteDeCalidadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AjusteDeCalidadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AjusteDeCalidad, AjusteDeCalidadDto>();
            Mapper.CreateMap<AjusteDeCalidadDto, AjusteDeCalidad>();
        }
    }
}
