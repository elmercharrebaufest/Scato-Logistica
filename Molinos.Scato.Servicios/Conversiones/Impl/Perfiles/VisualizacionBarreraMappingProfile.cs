using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class VisualizacionBarreraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VisualizacionBarreraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<VisualizacionBarrera, VisualizacionBarreraDto>()
                .ForMember(t => t.RolId, f => f.MapFrom(r => r.Rol.Id))
                .ForMember(t => t.RolDescripcion, f => f.MapFrom(r => r.Rol.Descripcion))
                .ForMember(x => x.SensoresBarreras, opt => opt.Ignore());
            Mapper.CreateMap<VisualizacionBarreraDto, VisualizacionBarrera>()
                .ForMember(x => x.SensoresBarreras, opt => opt.Ignore());
        }
    }
}
