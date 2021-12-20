using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DocumentoDeImpresionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DocumentoDeImpresionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DocumentoDeImpresion, DocumentoDeImpresionDto>();
            Mapper.CreateMap<DocumentoDeImpresionDto, DocumentoDeImpresion>();
        }
    }
}