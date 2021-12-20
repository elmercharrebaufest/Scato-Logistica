using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class VariedadPorVinedoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VariedadPorVinedoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<VariedadPorVinedo, VariedadPorVinedoDto>()
                .ForMember(t => t.Variedad, f => f.MapFrom(r => r.Variedad.Descripcion))
                .ForMember(t => t.VariedadId, f => f.MapFrom(r => r.Variedad.Id));
            Mapper.CreateMap<VariedadPorVinedoDto, VariedadPorVinedo>();
        }
    }
}