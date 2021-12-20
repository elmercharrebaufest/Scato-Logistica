using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class OrdenDeCargaContenedorMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "OrdenDeCargaContenedorMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<OrdenDeCargaContenedor, OrdenDeCargaContenedorDto>()
                  .ForMember(t => t.Transportista, f => f.MapFrom(r => r.Transportista.RazonSocial))
                  .ForMember(t => t.TransportistaId, f => f.MapFrom(r => r.Transportista.Id))
                  .ForMember(t => t.TipoComercial, f => f.MapFrom(r => r.TipoComercial.Descripcion))
                  .ForMember(t => t.TipoComercialId, f => f.MapFrom(r => r.TipoComercial.Id))
                  .ForMember(t => t.Chofer, f => f.MapFrom(r => r.Chofer))
                  .ForMember(t => t.ContenedorEntradaId, f => f.MapFrom(r => r.ContenedorEntrada.Id))
                  .ForMember(t => t.ContenedorSalidaId, f => f.MapFrom(r => r.ContenedorSalida.Id))
                  .ForMember(t => t.PesoContenedorEntrada, f => f.MapFrom(r => r.ContenedorEntrada.PesoTara))
                  .ForMember(t => t.PesoContenedorSalida, f => f.MapFrom(r => r.ContenedorSalida.PesoTara))
                  .ForMember(t => t.Material, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                  .ForMember(t => t.OrdenDeCargaContenedor, f => f.MapFrom(r => r.NroOrdenDeCargaContenedor))
                  .ForMember(t => t.DestinoId, f => f.MapFrom(r => r.Destino.Id))
                  .ForMember(t => t.DestinoCodigoSap, f => f.MapFrom(r => r.Destino.CodigoSap))
                  .ForMember(t => t.Destino, f => f.MapFrom(r => r.Destino.Descripcion))
                  .ForMember(t => t.Destino, f => f.MapFrom(r => r.Recorrido.Id))
                  .ForMember(t => t.TipoVehiculo, f => f.MapFrom(r => r.Recorrido.TipoVehiculo));
            Mapper.CreateMap<OrdenDeCargaContenedorDto, OrdenDeCargaContenedor>();

        }
    }
}
