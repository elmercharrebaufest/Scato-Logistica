using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RecorridoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RecorridoMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<Recorrido, RecorridoDto>()
                .ForMember(t => t.HidraulicasId,
                    f => f.MapFrom(r =>
                        r.PuestosDeCargaDescargas != null
                            ? r.PuestosDeCargaDescargas.Select(x => x.Id)
                            : new List<int>()))
                .ForMember(t => t.HidraulicasDesc,
                    f => f.MapFrom(r =>
                        r.PuestosDeCargaDescargas != null
                            ? String.Join(", ", r.PuestosDeCargaDescargas.Select(x => x.Nombre))
                            : ""))
                .ForMember(t => t.EsSustentable, f => f.MapFrom(r => r.Establecimiento != null))
                .ForMember(t => t.CalleDesc, f => f.MapFrom(r => r.Calle.Nombre))
                .ForMember(t => t.CalleId, f => f.MapFrom(r => r.Calle.Id))
                .ForMember(t => t.TieneFotoIngreso, f => f.MapFrom(r => r.Vehiculo != null && !string.IsNullOrEmpty(r.Vehiculo.CartaPorte.FotoRutaDestino)))
                .ForMember(t => t.TipoWorkFlowEgreso, f => f.MapFrom(r => r.Workflow != null && r.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso))
                .ForMember(t => t.TipoVariedadDescripcion, f => f.MapFrom(r => r.TipoVariedad.Descripcion))
                .ForMember(t => t.TipoVariedadCodigo, f => f.MapFrom(r => r.TipoVariedad.Codigo));
            Mapper.CreateMap<RecorridoDto, Recorrido>();

            Mapper.CreateMap<Recorrido, DatosInstanciaWorkflowDto>()
                .ForMember(t => t.Cuit, f => f.MapFrom(r => r.Chofer.Cuil))
                .ForMember(t => t.Id, f => f.MapFrom(r => r.InstanciaWorkflow))
                .ForMember(t => t.Material, f => f.MapFrom(r => r.Material.Descripcion))
                .ForMember(t => t.MaterialCodigoSap, f => f.MapFrom(r => r.Material.CodigoSAP))
                .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                .ForMember(t => t.Patente, f => f.MapFrom(r => r.Patente))
                .ForMember(t => t.Transportista, f => f.MapFrom(r => r.Transportista.RazonSocial))
                .ForMember(t => t.TransportistaId, f => f.MapFrom(r => r.Transportista.Id))
                .ForMember(t => t.PagaTicketMunicipal, f => f.MapFrom(r => r.PagaTicketMunicipal))
                .ForMember(t => t.EsGrano, f => f.MapFrom(r => r.Material.EsGrano))
                .ForMember(t => t.Entregador, f => f.MapFrom(r => r.Vehiculo.CartaPorte.Entregador.RazonSocial));

            Mapper.CreateMap<Recorrido, OtroRecorridoDelChoferDto>();
        }

    }
}