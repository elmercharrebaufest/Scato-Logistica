using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class UsuarioMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "UsuarioMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Usuario, UsuarioDto>();
            Mapper.CreateMap<Usuario, UsuarioMatriculaDto>();
            Mapper.CreateMap<UsuarioDto, Usuario>();
        }
    }
}
