using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DocumentoDeImpresionPorCentroMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DocumentoDeImpresionPorCentroMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DocumentoDeImpresionPorCentro, DocumentoDeImpresionPorCentroDto>()
                .ForMember(x => x.CentroDescripcion, x => x.MapFrom(t => t.Centro.Descripcion))
                .ForMember(x => x.CentroId, x => x.MapFrom(t => t.Centro.Id))
                .ForMember(x => x.PuestoDeTrabajoId, x => x.MapFrom(t => t.PuestoDeTrabajo.Id))
                .ForMember(x => x.PuestoDeTrabajoDescripcion, x => x.MapFrom(t => t.PuestoDeTrabajo.NombrePuesto))
                .ForMember(x => x.CentroDescripcion, x => x.MapFrom(t => t.Centro.Descripcion))
                .ForMember(x => x.ImpresoraDireccion, x => x.MapFrom(t => t.Impresora.Direccion))
                .ForMember(x => x.CodigoDocumentoImpresion, x => x.MapFrom(t => t.DocumentoDeImpresion.Codigo));

            Mapper.CreateMap<DocumentoDeImpresionPorCentroDto, DocumentoDeImpresionPorCentro>();
        }
    }
}