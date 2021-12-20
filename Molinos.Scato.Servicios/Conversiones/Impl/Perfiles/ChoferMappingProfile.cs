using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ChoferMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ChoferMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Chofer, ChoferDto>()
                .ForMember(t => t.TipoDocumentoIdentidadId, f => f.MapFrom(r => r.TipoDocumentoIdentidad.Id))
                .ForMember(t => t.TipoDocumentoIdentidadCodigoSap, f => f.MapFrom(r => r.TipoDocumentoIdentidad.CodigoSap))
                .ForMember(t => t.TipoDocumentoIdentidadDescripcion, f => f.MapFrom(r => r.TipoDocumentoIdentidad.Descripcion))
                .ForMember(t => t.DescripcionCorta, f => f.MapFrom(r => r.TipoDocumentoIdentidad.DescripcionCorta));
            Mapper.CreateMap<ChoferDto, Chofer>();
        }
    }
}