using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DescargaDeBinesMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DescargaDeBinesMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<DescargaDeBines, DescargaDeBinesDto>()
                  .ForMember(x => x.CuartelId, desc => desc.MapFrom(ddb => ddb.Cuartel.Id))
                  .ForMember(x => x.Cuartel, desc => desc.MapFrom(ddb => ddb.Cuartel.Codigo))
                  .ForMember(x => x.TipoId, desc => desc.MapFrom(ddb => ddb.Tipo.Id))
                  .ForMember(x => x.Tipo, desc => desc.MapFrom(ddb => ddb.Tipo.Descripcion))
                  .ForMember(x => x.Peso, desc => desc.MapFrom(ddb => ddb.Tipo.Peso))
                  .ForMember(x => x.EsGranel, desc => desc.MapFrom(ddb => ddb.Tipo.Clase.HasValue ? ddb.Tipo.Clase.Value == ClaseBin.Granel : (bool?)null));
            Mapper.CreateMap<DescargaDeBinesDto, DescargaDeBines>();
        }
    }
}
