using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CargaAnalisisCalidadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CargaAnalisisCalidadMappingProfile"; }
        }
        protected override void Configure()
        {

            Mapper.CreateMap<AnalisisDeCalidad, AnalisisDeCalidadDto>();
            Mapper.CreateMap<AnalisisDeCalidadDto, AnalisisDeCalidad>();
        }
    }
}
