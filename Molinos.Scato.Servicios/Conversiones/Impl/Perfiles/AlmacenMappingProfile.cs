using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AlmacenMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AlmacenMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Almacen, AlmacenDto>()
                .ForMember(x => x.CentroId, mat => mat.MapFrom(almacen => almacen.Centro.Id))
                .ForMember(x => x.EsSojaEPA, mat => mat.MapFrom(almacen => almacen.EPA));
            Mapper.CreateMap<AlmacenDto, Almacen>()
                .ForMember(x => x.EPA, mat => mat.MapFrom(almacen => almacen.EsSojaEPA));
        }
    }
}
