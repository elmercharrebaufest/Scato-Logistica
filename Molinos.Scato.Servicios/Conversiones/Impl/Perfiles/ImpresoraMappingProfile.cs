using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpresoraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpresoraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Impresora, ImpresoraDto>()
                  .ForMember(t => t.Descripcion, f => f.MapFrom(r => r.Descripcion))
                  .ForMember(t => t.Id, f => f.MapFrom(r => r.Id))
                  .ForMember(t => t.CentroId, f => f.MapFrom(r => r.Centro.Id))
                  .ForMember(t => t.IsZebra, f => f.MapFrom(r => r.IsZebra));
            Mapper.CreateMap<ImpresoraDto, Impresora>();
        }
    }
}