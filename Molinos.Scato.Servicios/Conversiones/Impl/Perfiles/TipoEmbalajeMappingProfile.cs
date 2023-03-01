using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TipoEmbalajeMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TipoEmbalajeMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TipoEmbalaje, TipoEmbalajeDto>();
            Mapper.CreateMap<TipoEmbalajeDto, TipoEmbalaje>();
        }
    }
}