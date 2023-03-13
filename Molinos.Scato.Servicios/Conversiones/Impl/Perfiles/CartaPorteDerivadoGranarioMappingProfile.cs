using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CartaPorteDerivadoGranarioMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CartaPorteDerivadoGranarioMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CartaPorteDerivadoGranario, CartaPorteDerivadoGranarioDto>()
                  .ForMember( x => x.RecorridoId, c => c.MapFrom(o => o.Recorrido.Id));
            Mapper.CreateMap<CartaPorteDerivadoGranarioDto, CartaPorteDerivadoGranario>();

        }
    }
}
