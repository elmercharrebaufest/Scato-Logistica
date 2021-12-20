using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TipoVehiculoBodegaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TipoVehiculoBodegaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TipoVehiculoBodega, TipoVehiculoBodegaDto>();
            Mapper.CreateMap<TipoVehiculoBodegaDto, TipoVehiculoBodega>();
        }
    }
}