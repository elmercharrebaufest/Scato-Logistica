using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpEtiquetaMuestraInaseMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpEtiquetaIntactaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpEtiquetaMuestraInase, ImpEtiquetaMuestraInaseDto>();
            Mapper.CreateMap<ImpEtiquetaMuestraInaseDto, ImpEtiquetaMuestraInase>();
        }
    }
}