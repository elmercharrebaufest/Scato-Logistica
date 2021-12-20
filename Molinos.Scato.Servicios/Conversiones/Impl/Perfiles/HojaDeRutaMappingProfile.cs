using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class HojaDeRutaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "HojaDeRutaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<HojaDeRuta, HojaDeRutaDto>()
                  .ForMember(x => x.MaterialId, c => c.MapFrom(o => o.Material.Id))
                  .ForMember(x => x.Material, c => c.MapFrom(o => o.Material.Descripcion))
                  .ForMember(x => x.TipoComercialId, c => c.MapFrom(o => o.TipoComercial.Id))
                  .ForMember(x => x.TipoComercial, c => c.MapFrom(o => o.TipoComercial.Descripcion))
                  .ForMember(x => x.TransportistaId, c => c.MapFrom(o => o.Transportista.Id))
                  .ForMember(x => x.Transportista, c => c.MapFrom(o => o.Transportista.RazonSocial))
                  .ForMember(x => x.TransportistaCuit, c => c.MapFrom(o => o.Transportista.Cuit))
                  .ForMember(x => x.TransportistaId, c => c.MapFrom(o => o.Transportista.Id))
                  .ForMember(x => x.MaterialCodigoSap, c => c.MapFrom(o => o.Material.CodigoSAP))
                  .ForMember(x => x.TipoVehiculo, c => c.MapFrom(o => o.Recorrido.TipoVehiculo));
            Mapper.CreateMap<HojaDeRutaDto, HojaDeRuta>();
        }
    }
}
