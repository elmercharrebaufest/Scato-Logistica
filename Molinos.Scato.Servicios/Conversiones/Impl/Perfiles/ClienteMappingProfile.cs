using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ClienteMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ClienteMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Cliente, ClienteDto>();
            Mapper.CreateMap<ClienteDto, Cliente>();
        }
    }
}
