using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EnvioUrenportMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EnvioUrenportMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<EnvioUrenport, EnvioUrenportDto>()
                .ForMember(x => x.RecorridoId, c => c.MapFrom(p => p.Recorrido.Id))
                .ForMember(x => x.Patente, c => c.MapFrom(p => p.Recorrido.Patente));
            Mapper.CreateMap<EnvioUrenportDto, EnvioUrenport>();
            
        }
    }
}