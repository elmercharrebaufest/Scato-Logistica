using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CamaraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CamaraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Camara, CamaraDto>()
                .ForMember(t => t.FormatoDeArchivo, f => f.MapFrom(r => r.FormatoDeArchivo.HasValue ? r.FormatoDeArchivo.Value : CamaraFormatoDeArchivo.NoEspecificado));
            Mapper.CreateMap<CamaraDto, Camara>();
        }
    }
}
