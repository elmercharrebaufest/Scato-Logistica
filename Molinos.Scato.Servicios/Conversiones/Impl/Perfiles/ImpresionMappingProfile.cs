using AutoMapper;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpresionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpresionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Dominio.Entidades.Impresion, ImpresionDto>();
            Mapper.CreateMap<ImpresionDto, Dominio.Entidades.Impresion>();
        }
    }
}