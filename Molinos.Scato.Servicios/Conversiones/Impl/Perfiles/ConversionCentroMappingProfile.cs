using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ConversionCentroMappingProfile : Profile
    {
        public override string ProfileName
        {
            get
            {
                return "ConversionCentroMappingProfile";
            }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<ConversionCentro, ConversionCentroDto>()
                  .ForMember(s => s.CentroDesc, f => f.MapFrom(t => t.Centro.Descripcion))
                  .ForMember(s => s.CentroId, f => f.MapFrom(t => t.Centro.Id))
                  .ForMember(s => s.CamaraId, f => f.MapFrom(t => t.Camara.Id))
                  .ForMember(s => s.CamaraDesc, f => f.MapFrom(t => t.Camara.Descripcion));
            Mapper.CreateMap<ConversionCentroDto, ConversionCentro>();
        }
    }
}
