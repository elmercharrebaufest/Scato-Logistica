using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MuestraDeHumedadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MuestraDeHumedadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MuestraDeHumedad, MuestraDeHumedadDto>()
                .ForMember(t => t.HumedimetroId, f => f.MapFrom(r => r.Humedimetro.Id))
                .ForMember(t => t.Humedimetro, f => f.MapFrom(r => r.Humedimetro.Descripcion))
                .ForMember(t => t.Centro, f => f.MapFrom(r => r.Centro.CodigoSAP))
                .ForMember(t => t.CentroId, f => f.MapFrom(r => r.Centro.Id));
            Mapper.CreateMap<MuestraDeHumedadDto, MuestraDeHumedad>();
        }
    }
}
