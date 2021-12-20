using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpEtiquetaRubrosAnalizarMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpEtiquetaRubrosAnalizarMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<ImpEtiquetaRubrosAnalizar, ImpEtiquetaRubrosAnalizarDto>();
            Mapper.CreateMap<ImpEtiquetaRubrosAnalizarDto, ImpEtiquetaRubrosAnalizar>();
        }
    }
}
