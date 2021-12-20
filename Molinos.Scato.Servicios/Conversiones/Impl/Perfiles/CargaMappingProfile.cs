using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CargaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CargaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Carga, CargaDto>()
                .ForMember(t => t.Bodega, f => f.MapFrom(r => r.Bodega.Nombre))
                .ForMember(t => t.Destino, f => f.MapFrom(r => r.Destino.Nombre))
                .ForMember(t => t.Exportador, f => f.MapFrom(r => r.Exportador.Nombre))
                .ForMember(t => t.Material, f => f.MapFrom(r => r.Material.Descripcion))
                .ForMember(t => t.Vapor, f => f.MapFrom(r => r.Vapor.Nombre))
                .ForMember(t => t.Tipo, f => f.MapFrom(r => r.Tipo));
            Mapper.CreateMap<CargaDto, Carga>();
        }
    }
}