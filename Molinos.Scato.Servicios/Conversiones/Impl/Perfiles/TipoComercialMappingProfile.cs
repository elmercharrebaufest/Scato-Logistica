using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TipoComercialMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TipoComercialMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TipoComercial, TipoComercialDto>();
            Mapper.CreateMap<TipoComercialDto, TipoComercial>();
        }
    }
}