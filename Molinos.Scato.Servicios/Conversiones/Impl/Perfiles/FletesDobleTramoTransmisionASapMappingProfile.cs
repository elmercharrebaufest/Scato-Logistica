using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class FletesDobleTramoTransmisionASapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "FletesDobleTramoTransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<FletesDobleTramoTransmisionASap, FletesDobleTramo>();
            Mapper.CreateMap<FletesDobleTramo, FletesDobleTramoTransmisionASap>();
        }
    }
}
