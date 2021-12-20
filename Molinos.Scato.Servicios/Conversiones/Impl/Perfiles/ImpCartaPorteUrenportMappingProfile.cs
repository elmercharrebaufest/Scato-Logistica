using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpCartaPorteUrenportMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpCartaPorteUrenportMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpCartaPorteUrenport, ImpCartaPorteUrenportDto>();
            Mapper.CreateMap<ImpCartaPorteUrenportDto, ImpCartaPorteUrenport>();
        }
    }
}