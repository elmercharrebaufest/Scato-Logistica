using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LineUpMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LineUpMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LineUp, LineUpDto>().ForMember(x => x.InstanciaWorkflow,
                x => x.MapFrom(y => y.Recorrido.InstanciaWorkflow));
            Mapper.CreateMap<LineUpDto, LineUp>();
        }
    }
}
