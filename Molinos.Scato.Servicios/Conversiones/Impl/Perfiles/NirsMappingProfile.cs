using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class NirsMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "NirsMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Nirs, NirsDto>()
                .ForMember(x => x.CentroId, h => h.MapFrom(nirs => nirs.Centro.Id)); ;
            Mapper.CreateMap<NirsDto, Nirs>();
        }
    }
}
