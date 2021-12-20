using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CartaPorteElectronicaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CartaPorteElectronicaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CartaPorteElectronica, CartaPorteElectronicaDto>();
            Mapper.CreateMap<CartaPorteElectronicaDto, CartaPorteElectronica>();
            Mapper.CreateMap<CartaPorteElectronica, CartaPorteElectronica>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Pdf, opt => opt.Ignore());
        }
    }
}
