using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TalonarioMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TalonarioMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Talonario, TalonarioDto>()
                .ForMember(x => x.CentroId, mat => mat.MapFrom(talonario => talonario.Centro.Id));
            Mapper.CreateMap<TalonarioDto, Talonario>();
        }
    }
}
