using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CaladoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CaladoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Calado, CaladoDto>()
                .ForMember(x => x.CalidadMaterialId, cxc => cxc.MapFrom(x => x.CalidadMaterial.Id))
                .ForMember(x => x.CalidadMaterialDescripcion, cxc => cxc.MapFrom(x => x.CalidadMaterial.Descripcion));
            Mapper.CreateMap<CaladoDto, Calado>();
        }
    }
}
