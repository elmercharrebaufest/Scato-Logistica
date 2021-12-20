using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class FormatoDePapelMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "FormatoDePapelMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<FormatoDePapel, FormatoDePapelDto>()
                  .ForMember(t => t.Descripcion, f => f.MapFrom(r => r.Descripcion))
                  .ForMember(t => t.Id, f => f.MapFrom(r => r.Id));
            Mapper.CreateMap<FormatoDePapelDto, FormatoDePapel>();
        }
    }
}