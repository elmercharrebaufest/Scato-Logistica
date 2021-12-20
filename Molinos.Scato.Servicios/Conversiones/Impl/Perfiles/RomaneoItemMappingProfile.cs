using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RomaneoItemMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RomaneoItemMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<RomaneoItem, RomaneoItemDto>()
                  .ForMember(t => t.Almacen, f => f.MapFrom(r => r.Almacen.Descripcion))
                  .ForMember(t => t.AlmacenId, f => f.MapFrom(r => r.Almacen.Id))
                  .ForMember(t => t.Material, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                  .ForMember(t => t.MaterialCodigoSap, f => f.MapFrom(r => r.Material.CodigoSAP))
                  .ForMember(t => t.RomaneoId, f => f.MapFrom(r => r.Romaneo.Id))
                  .ForMember(t => t.BalanzaId, f => f.MapFrom(r => r.Balanza.Id))
                  .ForMember(t => t.ModalidadBalanza, f => f.MapFrom(r => r.Balanza.TipoBalanza))
                  .ForMember(t => t.TaraRomaneoId, f => f.MapFrom(r => r.TaraRomaneo.Id));
            Mapper.CreateMap<RomaneoItemDto, RomaneoItem>();
        }
    }
}