using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class BocaDestinoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "BocaDestinoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<BocaDestino, BocaDestinoDto>()
                  .ForMember(t => t.Localidad ,f => f.MapFrom(r => r.Localidad.Descripcion))
                  .ForMember(t => t.LocalidadId, f => f.MapFrom(r => r.Localidad.Id))
                  .ForMember(t => t.Provincia, f => f.MapFrom(r => r.Provincia.Descripcion))
                  .ForMember(t => t.Proveedor, f => f.MapFrom(r => r.Proveedor.Descripcion))
                  .ForMember(t => t.ProvinciaId, f => f.MapFrom(r => r.Provincia.Id));
            Mapper.CreateMap<BocaDestinoDto, BocaDestino>();
        }
    }
}