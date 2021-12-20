using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpFormulario239MappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpFormulario239MappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpFormulario239, ImpFormulario239Dto>();
            Mapper.CreateMap<ImpFormulario239Dto, ImpFormulario239>();
        }
    }
}
