using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class VagonMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VagonMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Vagon, vagon>();
            Mapper.CreateMap<vagon, Vagon>();
        }
    }
}
