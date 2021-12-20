using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PesoMaximoPorTipoVehiculoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PesoMaximoPorTipoVehiculoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<PesoMaximoPorTipoVehiculo, PesoMaximoPorTipoVehiculoDto>()
                .ForMember(t => t.CentroId, f => f.MapFrom(r => r.Centro.Id));
            Mapper.CreateMap<PesoMaximoPorTipoVehiculoDto, PesoMaximoPorTipoVehiculo>();
        }
    }
}
