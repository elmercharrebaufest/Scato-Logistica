using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DocumentoPorRecorridoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DocumentoPorRecorridoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DocumentoPorRecorrido, DocumentoPorRecorridoDto>();                
            Mapper.CreateMap<DocumentoPorRecorridoDto, DocumentoPorRecorrido>();
        }
    }
}