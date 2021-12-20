using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MuestraDeNirsMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MuestraDeNirsMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MuestraDeNirs, MuestraDeNirsDto>()
                .ForMember(t => t.NirsId, f => f.MapFrom(r => r.Nirs.Id))
                .ForMember(t => t.Nirs, f => f.MapFrom(r => r.Nirs.Descripcion))
                .ForMember(t => t.Centro, f => f.MapFrom(r => r.Centro.CodigoSAP))
                .ForMember(t => t.CentroId, f => f.MapFrom(r => r.Centro.Id));
            Mapper.CreateMap<MuestraDeNirsDto, MuestraDeNirs>();
        }
    }
}
