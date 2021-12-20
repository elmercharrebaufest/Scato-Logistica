using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RomaneoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RomaneoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Romaneo, RomaneoDto>()
                .ForMember(t => t.ProveedorDescripcion, f => f.MapFrom(r => r.Proveedor.Descripcion))
                .ForMember(t => t.ProveedorId, f => f.MapFrom(r => r.Proveedor.Id))
                .ForMember(t => t.Numero, f => f.MapFrom(r => r.Id));
            Mapper.CreateMap<RomaneoDto, Romaneo>();
        }
    }
}