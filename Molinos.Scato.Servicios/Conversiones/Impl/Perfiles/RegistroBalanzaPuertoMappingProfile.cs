using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RegistroBalanzaPuertoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RegistroBalanzaPuertoMappingProfile"; }
        }
        protected override void Configure()
        {

            Mapper.CreateMap<RegistroBalanzaPuerto, RegistroBalanzaPuertoDto>();
            Mapper.CreateMap<RegistroBalanzaPuertoDto, RegistroBalanzaPuerto>();
        }
    }
}
