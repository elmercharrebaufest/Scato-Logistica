using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System.Linq;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class BajaCTGMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "BajaCTGMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<BajaCTG, BajaCTGDto>();
            Mapper.CreateMap<BajaCTGDto, BajaCTG>();

            Mapper.CreateMap<BajaCTG, BajaCTGRetransmisionDto>()
                .ForMember(x => x.EstadoCtg, c => c.MapFrom(p => (p.CodigoDeBaja == null) ? EstadoTransmisionASap.Error : EstadoTransmisionASap.Correcto))
                .ForMember(x => x.EstadoCtgDefinitivo, c => c.MapFrom(p => (p.CodigoDeBajaDefinitivo == null) ? EstadoTransmisionASap.Error : EstadoTransmisionASap.Correcto))
                .ForMember(x => x.Dto, c => c.MapFrom(p => p.CartaPorte))
                .ForMember(x => x.Vehiculo, c => c.MapFrom(p => new VehiculoDto { PesoNetoOrigen = p.CartaPorte.Vehiculos.First().PesoNetoOrigen, Patente = p.CartaPorte.Vehiculos.First().Patente }))
                .ForMember(x => x.CentroId, c => c.MapFrom(p => p.CartaPorte.CentroDestino.Id))
                .ForMember(x => x.WorkflowId, c => c.MapFrom(p => p.WorkflowId))
                .ForMember(x => x.OrdenDeDescargaFason, c => c.MapFrom(p => p.OrdenDeDescargaFason))
                .ForMember(x => x.TipoDocumentoIngreso, c => c.MapFrom(p => (p.OrdenDeDescargaFason != null) ? TipoDocumentoIngreso.OrdenDeDescargaFason : TipoDocumentoIngreso.CartaPorte));
        }
    }
}