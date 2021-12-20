using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CentroMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CentroMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Centro, CentroDto>()
                .ForMember(x => x.CamaraId, c => c.MapFrom(centro => centro.CamaraDefault.Id))
                .ForMember(x => x.CamaraDesc, c => c.MapFrom(centro => centro.CamaraDefault.Descripcion))
                .ForMember(x => x.CamaraCodigoSap, c => c.MapFrom(centro => centro.CamaraDefault.CodigoSAP))
                .ForMember(x => x.ProvinciaId, c => c.MapFrom(centro => centro.Provincia.Id))
                .ForMember(x => x.ProvinciaDesc, c => c.MapFrom(centro => centro.Provincia.Descripcion))
                .ForMember(x => x.LocalidadId, c => c.MapFrom(centro => centro.Localidad.Id))
                .ForMember(x => x.LocalidadCodigoSap, c => c.MapFrom(centro => centro.Localidad.CodigoAfip))
                .ForMember(x => x.LocalidadDesc, c => c.MapFrom(centro => centro.Localidad.Descripcion));
            Mapper.CreateMap<CentroDto, Centro>();
        }
    }
}
