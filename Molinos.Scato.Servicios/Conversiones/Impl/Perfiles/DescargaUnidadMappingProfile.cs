using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DescargaUnidadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DescargaUnidadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DescargaUnidad, DescargaUnidadDto>()
                .ForMember(t => t.ProveedorDescripcion, f => f.MapFrom(r => r.Proveedor.Descripcion))
                .ForMember(t => t.ProveedorId, f => f.MapFrom(r => r.Proveedor.Id))
                .ForMember(t => t.NroDescarga, f => f.MapFrom(r => r.Id));
            Mapper.CreateMap<DescargaUnidadDto, DescargaUnidad>();
        }
    }
}