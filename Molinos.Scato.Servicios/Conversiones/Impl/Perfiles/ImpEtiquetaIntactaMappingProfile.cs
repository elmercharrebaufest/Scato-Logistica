using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpEtiquetaIntactaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpEtiquetaIntactaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpEtiquetaIntacta, ImpEtiquetaIntactaDto>();
            Mapper.CreateMap<ImpEtiquetaIntactaDto, ImpEtiquetaIntacta>();
        }
    }
}