using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class SuplenciaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "SuplenciaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Suplencia, SuplenciaDto>()
                .ForMember(x => x.UsuarioASuplantarId, mat => mat.MapFrom(suplencia => suplencia.UsuarioASuplantar.Id))
                .ForMember(x => x.UsuarioASuplantarNombreUsuario, mat => mat.MapFrom(suplencia => suplencia.UsuarioASuplantar.NombreUsuario))
                .ForMember(x => x.UsuarioSuplenteId, mat => mat.MapFrom(suplencia => suplencia.UsuarioSuplente.Id))
                .ForMember(x => x.UsuarioSuplenteNombreUsuario, mat => mat.MapFrom(suplencia => suplencia.UsuarioSuplente.NombreUsuario));
            Mapper.CreateMap<SuplenciaDto, Suplencia>();
        }
    }
}
