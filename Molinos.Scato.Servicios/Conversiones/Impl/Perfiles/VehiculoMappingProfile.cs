using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class VehiculoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VehiculoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Vehiculo, VehiculoDto>()
                .ForMember(x => x.CartaPorteId, c => c.MapFrom(w => w.CartaPorte.Id));
            Mapper.CreateMap<VehiculoDto, Vehiculo>();
        }
    }
}
