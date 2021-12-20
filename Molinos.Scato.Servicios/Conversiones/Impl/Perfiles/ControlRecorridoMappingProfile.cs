using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ControlRecorridoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ControlRecorridoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ControlRecorrido, ControlRecorridoDto>();
            Mapper.CreateMap<ControlRecorridoDto, ControlRecorrido>();
        }
    }
}
