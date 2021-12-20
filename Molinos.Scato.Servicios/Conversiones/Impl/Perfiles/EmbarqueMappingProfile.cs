using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EmbarqueMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EmbarqueMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Embarque, EmbarqueDto>().ForMember(x => x.InstanciaWorkflow,
                x => x.MapFrom(y => y.Recorrido.InstanciaWorkflow))
                .ForMember(x => x.NombreBuque,
                x => x.MapFrom(y => y.Vapor.Nombre))
                .ForMember(x => x.MaterialesPuertoCantidad,
                x => x.MapFrom(y => y.MaterialPuertoCantidad));
            Mapper.CreateMap<EmbarqueDto, Embarque>();
        }
    }
}
