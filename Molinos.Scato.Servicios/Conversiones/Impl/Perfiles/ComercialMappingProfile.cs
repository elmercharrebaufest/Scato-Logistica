using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ComercialMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ComercialMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Comercial, ComercialDto>();
            Mapper.CreateMap<ComercialDto, Comercial>();
        }
    }
}
