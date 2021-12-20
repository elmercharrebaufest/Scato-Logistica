using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class IngresoDeDatosDeExportacionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "IngresoDeDatosDeExportacionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<IngresoDeDatosDeExportacion, IngresoDeDatosDeExportacionDto>()
                .ForMember(t => t.FirmaRazonSocial, f => f.MapFrom(r => r.Firma.RazonSocial))
                .ForMember(t => t.FirmaCuit, f => f.MapFrom(r => r.Firma.Cuit))
                .ForMember(t => t.InstanciaWorkflow, f => f.MapFrom(r => r.Recorrido.InstanciaWorkflow))
                .ForMember(t => t.Nacionalidad, f => f.MapFrom(r => r.Nacionalidad.Descripcion))
                .ForMember(t => t.Transportista, f => f.MapFrom(r => r.Transportista.RazonSocial));
            Mapper.CreateMap<IngresoDeDatosDeExportacionDto, IngresoDeDatosDeExportacion>();
        }
    }
}