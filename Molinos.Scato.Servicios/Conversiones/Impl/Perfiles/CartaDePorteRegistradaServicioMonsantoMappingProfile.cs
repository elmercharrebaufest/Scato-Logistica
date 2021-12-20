using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CartaDePorteRegistradaServicioMonsantoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CartaDePorteRegistradaServicioMonsantoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CartaDePorteRegistradaServicioMonsanto, CartaDePorteRegistradaServicioMonsantoDto>();
            Mapper.CreateMap<CartaDePorteRegistradaServicioMonsantoDto, CartaDePorteRegistradaServicioMonsanto>();
        }
    }
}