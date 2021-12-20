using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class FormatoDeCampoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "FormatoDeCampoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<FormatoDeCampo, FormatoDeCampoDto>()
                .ForMember(t => t.Id, f => f.MapFrom(r => r.Id))
                .ForMember(t => t.CampoDescripcion, f => f.MapFrom(r => r.Campo.Descripcion))
                .ForMember(t => t.CampoDireccion, f => f.MapFrom(r => r.Campo.Direccion))
                .ForMember(t => t.TituloOncca, f => f.MapFrom(r => r.Campo.TituloOncca))
                .ForMember(t => t.CampoId, f => f.MapFrom(r => r.Campo.Id))
                .ForMember(t => t.LetraDescripcion, f => f.MapFrom(r => r.Letra.Descripcion))
                .ForMember(t => t.LetraId, f => f.MapFrom(r => r.Letra.Id))
                .ForMember(t => t.Texto, f => f.MapFrom(r => r.Texto));
            Mapper.CreateMap<FormatoDeCampoDto, FormatoDeCampo>();
        }
    }
}