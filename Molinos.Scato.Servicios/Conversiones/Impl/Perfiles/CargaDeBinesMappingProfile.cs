using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CargaDeBinesMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CargaDeBinesMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CargaDeBines, CargaDeBinesDto>()
                  .ForMember(x => x.TipoId, desc => desc.MapFrom(ddb => ddb.Tipo.Id))
                  .ForMember(x => x.Tipo, desc => desc.MapFrom(ddb => ddb.Tipo.Descripcion));
            Mapper.CreateMap<CargaDeBinesDto, CargaDeBines>();
        }
    }
}
