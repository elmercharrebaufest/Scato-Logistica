using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class InhabilitacionChoferMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "InhabilitacionChoferMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<InhabilitacionChofer, InhabilitacionChoferDto>()
                .ForMember(t => t.TipoDocumentoIdentidadId, f => f.MapFrom(r => r.Chofer.TipoDocumentoIdentidad.Id))
                .ForMember(t => t.DescripcionCorta, f => f.MapFrom(r => r.Chofer.TipoDocumentoIdentidad.DescripcionCorta))
                .ForMember(t => t.Apellido, f => f.MapFrom(r => r.Chofer.Apellido))
                .ForMember(t => t.Nombre, f => f.MapFrom(r => r.Chofer.Nombre))
                .ForMember(t => t.NumeroDeDocumento, f => f.MapFrom(r => r.Chofer.NumeroDeDocumento))
                .ForMember(t => t.Adjuntos, f => f.MapFrom(r => r.Adjuntos));
            Mapper.CreateMap<InhabilitacionChoferDto, InhabilitacionChofer>();
        }
    }
}