using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TipoDocumentoIdentidadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TipoDocumentoIdentidadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TipoDocumentoIdentidad, TipoDocumentoIdentidadDto>();
            Mapper.CreateMap<TipoDocumentoIdentidadDto, TipoDocumentoIdentidad>();
        }
    }
}