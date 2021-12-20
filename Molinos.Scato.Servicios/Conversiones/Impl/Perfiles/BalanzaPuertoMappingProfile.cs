using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class BalanzaPuertoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "BalanzaPuertoMappingProfile"; }
        }
        protected override void Configure()
        {

            Mapper.CreateMap<BalanzaPuerto, BalanzaPuertoDto>();
            Mapper.CreateMap<BalanzaPuertoDto, BalanzaPuerto>();
        }
    }
}
