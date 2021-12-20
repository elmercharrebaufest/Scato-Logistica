using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpDeclaracionFosfinaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpDeclaracionFosfinaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpDeclaracionFosfina, ImpDeclaracionFosfinaDto>();
            Mapper.CreateMap<ImpDeclaracionFosfinaDto, ImpDeclaracionFosfina>();
        }
    }
}
