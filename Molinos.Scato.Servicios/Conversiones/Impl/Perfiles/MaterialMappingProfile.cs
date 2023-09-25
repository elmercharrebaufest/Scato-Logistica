using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MaterialMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MaterialMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Material, MaterialDto>()
                    .ForMember(x => x.AlmacenOrigenId, mat => mat.MapFrom(material => material.AlmacenOrigen.Id))
                    .ForMember(x => x.AlmacenOrigenDesc, mat => mat.MapFrom(material => material.AlmacenOrigen.Descripcion))
                    .ForMember(x => x.TipoVariedadId, mat => mat.MapFrom(material => material.Variedad.Id))
                    .ForMember(x => x.VariedadDesc, mat => mat.MapFrom(material => material.Variedad.Descripcion))
                    .ForMember(x => x.TipoEmbalajeId, mat => mat.MapFrom(material => material.TipoEmbalaje.Id));
            Mapper.CreateMap<MaterialDto, Material>();
        }
    }
}
