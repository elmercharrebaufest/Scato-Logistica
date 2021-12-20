using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RemitoBodegaVinoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RemitoBodegaVinoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<RemitoBodegaVino, RemitoBodegaVinoDto>()
                  .ForMember(x => x.RecorridoId, cxc => cxc.MapFrom(x => x.Recorrido.Id))
                  .ForMember(x => x.TipoComercialId, cxc => cxc.MapFrom(x => x.TipoComercial.Id))
                  .ForMember(x => x.TipoComercial, cxc => cxc.MapFrom(x => x.TipoComercial.Descripcion))
                  .ForMember(x => x.TransportistaId, cxc => cxc.MapFrom(x => x.Transportista.Id))
                  .ForMember(x => x.Transportista, cxc => cxc.MapFrom(x => x.Transportista.RazonSocial))
                  .ForMember(x => x.ProveedorId, cxc => cxc.MapFrom(x => x.Proveedor.Id))
                  .ForMember(x => x.Proveedor, cxc => cxc.MapFrom(x => x.Proveedor.Descripcion))
                  .ForMember(x => x.VariedadId, cxc => cxc.MapFrom(x => x.Material.Variedad.Id))
                  .ForMember(x => x.Variedad, cxc => cxc.MapFrom(x => x.Material.Variedad.Descripcion))
                  .ForMember(x => x.TipoVehiculoBodegaId, cxc => cxc.MapFrom(x => x.TipoVehiculoBodega.Id))
                  .ForMember(x => x.TipoVehiculoBodega, cxc => cxc.MapFrom(x => x.TipoVehiculoBodega.Descripcion))
                  .ForMember(x => x.Chofer, cxc => cxc.MapFrom(x => x.Chofer))
                  .ForMember(x => x.Material, cxc => cxc.MapFrom(x => x.Material.Descripcion))
                  .ForMember(x => x.MaterialId, cxc => cxc.MapFrom(x => x.Material.Id))
                  .ForMember(x => x.MaterialCodigoSap, cxc => cxc.MapFrom(x => x.Material.CodigoSAP));
            Mapper.CreateMap<RemitoBodegaVinoDto, RemitoBodegaVino>();

            Mapper.CreateMap<RemitoBodegaVino, ListadoDePesadasDto>()
               .ForMember(t => t.RemitoSAP, f => f.MapFrom(r => r.NroRemito))
               .ForMember(t => t.Material, f => f.MapFrom(r => r.Material.Descripcion));
        }
    }
}
