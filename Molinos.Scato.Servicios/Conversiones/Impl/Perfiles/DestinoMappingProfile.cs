using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class DestinoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DestinoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Destino, DestinoDto>();
            Mapper.CreateMap<DestinoDto, Destino>();
        }
    }
}