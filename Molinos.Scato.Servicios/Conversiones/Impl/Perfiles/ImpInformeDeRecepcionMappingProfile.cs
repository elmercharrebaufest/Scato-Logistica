using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpInformeDeRecepcionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpInformeDeRecepcionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpInformeDeRecepcion, ImpInformeDeRecepcionDto>();
            Mapper.CreateMap<ImpInformeDeRecepcionDto, ImpInformeDeRecepcion>();
        }
    }
}
