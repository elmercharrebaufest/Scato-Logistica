using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LocalidadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LocalidadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Localidad, LocalidadDto>()
                  .ForMember(x => x.CodigoAfip, c => c.MapFrom(localidad => localidad.CodigoAfip))
                  .ForMember(x => x.ProvinciaId, c => c.MapFrom(localidad => localidad.Provincia.Id))
                  .ForMember(x => x.ProvinciaDesc, c => c.MapFrom(localidad => localidad.Provincia.Descripcion));
            Mapper.CreateMap<LocalidadDto, Localidad>();
        }
    }
}
