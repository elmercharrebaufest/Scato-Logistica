using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

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
                .ForMember(t => t.TieneFotoIngreso, f => f.MapFrom(r => r.Vehiculo != null && !string.IsNullOrEmpty(r.Vehiculo.CartaPorte.FotoRutaDestino)));
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
                .ForMember(t => t.PagaTicketMunicipal, f => f.MapFrom(r => r.PagaTicketMunicipal));

            Mapper.CreateMap<Recorrido, OtroRecorridoDelChoferDto>();
        }

    }
}