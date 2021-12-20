using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CuartelMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CuartelMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Cuartel, CuartelDto>()
                .ForMember(t => t.VinedoPropio, f => f.MapFrom(r => r.VinedoPropio.Descripcion))
                .ForMember(t => t.VinedoPropioId, f => f.MapFrom(r => r.VinedoPropio.Id));
            Mapper.CreateMap<CuartelDto, Cuartel>();
        }
    }
}
