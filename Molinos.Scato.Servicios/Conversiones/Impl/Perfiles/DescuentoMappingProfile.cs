using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DescuentoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DescuentoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Descuento, DescuentoDto>();
            Mapper.CreateMap<DescuentoDto, Descuento>();
        }
    }
}