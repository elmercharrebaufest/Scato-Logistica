using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MaterialPuertoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MaterialPuertoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MaterialPuerto, MaterialPuertoDto>()
            .ForMember(t => t.AlmacenDesc, f => f.MapFrom(r => r.Almacen.Descripcion))
            .ForMember(t => t.Almacen_Id, f => f.MapFrom(r => r.Almacen.Id));
            Mapper.CreateMap<MaterialPuertoDto, MaterialPuerto>();
        }
    }
}
