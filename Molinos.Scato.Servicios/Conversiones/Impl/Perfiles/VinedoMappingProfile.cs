using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class VinedoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VinedoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Vinedo, VinedoDto>()
                .ForMember(t => t.Descripcion, f => f.MapFrom(r => r.Descripcion));
            Mapper.CreateMap<VinedoDto, Vinedo>();
        }
    }
}