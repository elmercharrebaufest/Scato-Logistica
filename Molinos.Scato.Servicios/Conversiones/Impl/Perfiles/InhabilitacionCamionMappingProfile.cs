using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class InhabilitacionCamionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "InhabilitacionCamionMappingProfile"; }
        }
        protected override void Configure()
        {
            //Solo mapeo los campos que estan dentro de otros objetos
            Mapper.CreateMap<InhabilitacionCamion, InhabilitacionCamionDto>()
                .ForMember(t => t.Adjuntos, f => f.MapFrom(r => r.Adjuntos));
            Mapper.CreateMap<InhabilitacionCamionDto, InhabilitacionCamion>();
        }
    }
}