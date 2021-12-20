using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ATAPuertoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ATAPuertoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ATAPuerto, ATAPuertoDto>();
            Mapper.CreateMap<ATAPuertoDto, ATAPuerto>();
        }
    }
}