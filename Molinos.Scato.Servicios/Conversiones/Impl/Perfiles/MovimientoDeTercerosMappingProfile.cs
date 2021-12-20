using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MovimientoDeTercerosMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MovimientoDeTercerosMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MovimientoDeTerceros, MovimientoDeTercerosDto>();
            Mapper.CreateMap<MovimientoDeTercerosDto, MovimientoDeTerceros>();
        }
    }
}