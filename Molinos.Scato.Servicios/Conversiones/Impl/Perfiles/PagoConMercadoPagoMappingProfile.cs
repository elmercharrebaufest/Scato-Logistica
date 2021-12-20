using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PagoConMercadoPagoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PagoConMercadoPagoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<PagoConMercadoPago, PagoConMercadoPagoDto>();
            Mapper.CreateMap<PagoConMercadoPagoDto, PagoConMercadoPago>();
        }
    }
}