using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DocumentoExternoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DocumentoExternoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DocumentoExterno, DocumentoExternoDto>()
                 .ForMember(t => t.ArchivoRutaDestino, f => f.MapFrom(r => r.ArchivoRutaDestino))
                 .ForMember(t => t.ArchivoExtension, f => f.MapFrom(r => r.ArchivoExtension))
                 .ForMember(t => t.NumeroDeDocumento, f => f.MapFrom(r => r.NumeroDeDocumento));
            Mapper.CreateMap<DocumentoExternoDto, DocumentoExterno>();
        }
    }
}