using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class FormatoDeImpresionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "FormatoDeImpresionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<FormatoDeImpresion, FormatoDeImpresionDto>()
                .ForMember(t => t.FormatoDePapelId, f => f.MapFrom(r => r.FormatoDePapel.Id))
                  .ForMember(t => t.FormatoDePapelDescripcion, f => f.MapFrom(r => r.FormatoDePapel.Descripcion))
                  .ForMember(t => t.FormatoDePapelAlto, f => f.MapFrom(r => r.FormatoDePapel.Alto))
                  .ForMember(t => t.FormatoDePapelAncho, f => f.MapFrom(r => r.FormatoDePapel.Ancho));
            Mapper.CreateMap<FormatoDeImpresionDto, FormatoDeImpresion>();
        }
    }
}