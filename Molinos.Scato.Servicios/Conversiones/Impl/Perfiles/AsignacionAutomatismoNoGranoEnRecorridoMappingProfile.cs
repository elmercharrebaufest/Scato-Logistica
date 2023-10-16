using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AsignacionAutomatismoNoGranoEnRecorridoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AsignacionAutomatismoNoGranoEnRecorridoMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<AsignacionAutomatismoNoGranoEnRecorrido, AsignacionAutomatismoNoGranoEnRecorridoDto>()
                .ReverseMap();
        }
    }
}