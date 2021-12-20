using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EntregadorMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EntregadorMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Entregador, EntregadorDto>()
                  .ForMember(f => f.PaisId, t => t.MapFrom(s => s.Pais.Id))
                  .ForMember(f => f.Pais, t => t.MapFrom(s => s.Pais.Descripcion))
                  .ForMember(f => f.ProvinciaId, t => t.MapFrom(s => s.Provincia.Id))
                  .ForMember(f => f.Provincia, t => t.MapFrom(s => s.Provincia.Descripcion))
                  .ForMember(f => f.LocalidadId, t => t.MapFrom(s => s.Localidad.Id))
                  .ForMember(f => f.Localidad, t => t.MapFrom(s => s.Localidad.Id));
            Mapper.CreateMap<EntregadorDto, Entregador>();
        }
    }
}
