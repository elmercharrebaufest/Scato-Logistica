using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpImpresionGenericaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpImpresionGenericaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpImpresionGenerica, ImpImpresionGenericaDto>();
            Mapper.CreateMap<ImpImpresionGenericaDto, ImpImpresionGenerica>();
        }
    }
}
