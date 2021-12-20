using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DistribucionDeAlmacenMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DistribucionDeAlmacenMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DistribucionDeAlmacen, DistribucionDeAlmacenDto>()
                .ForMember(t => t.Almacen, f => f.MapFrom(r => r.Almacen.Descripcion))
                .ForMember(t => t.AlmacenId, f => f.MapFrom(r => r.Almacen.Id))
                .ForMember(t => t.AlmacenSap, f => f.MapFrom(r => r.Almacen.CodigoSAP));
            Mapper.CreateMap<DistribucionDeAlmacenDto, DistribucionDeAlmacen>()
                .ForMember(t => t.Almacen, f => f.MapFrom(r => (Almacen)null));
        }
    }
}