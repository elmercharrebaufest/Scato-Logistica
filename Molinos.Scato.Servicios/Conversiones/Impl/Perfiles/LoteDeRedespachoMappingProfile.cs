using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LoteDeRedespachoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LoteDeRedespachoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LoteDeRedespacho, LoteDeRedespachoDto>();
            Mapper.CreateMap<LoteDeRedespachoDto, LoteDeRedespacho>();
        }
    }
}