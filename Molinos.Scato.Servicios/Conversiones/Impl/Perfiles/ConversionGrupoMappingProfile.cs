using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ConversionGrupoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ConversionGrupoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ConversionGrupo, ConversionGrupoDto>()
                  .ForMember(t => t.CodigoSegunCamara, f => f.MapFrom(r => r.CodigoSegunCamara))
                  .ForMember(t => t.CamaraDesc, f => f.MapFrom(r => r.Camara.Descripcion))
                  .ForMember(t => t.CamaraId, f => f.MapFrom(r => r.Camara.Id))
                  .ForMember(t => t.MaterialDesc, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id));
            Mapper.CreateMap<ConversionGrupoDto, ConversionGrupo>();
        }
    }
}