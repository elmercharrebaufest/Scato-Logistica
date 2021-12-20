using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RamalFerroviarioMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RamalFerroviarioMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<RamalFerroviario, RamalFerroviarioDto>();

            Mapper.CreateMap<RamalFerroviarioDto, RamalFerroviario>();
        }
    }
}
