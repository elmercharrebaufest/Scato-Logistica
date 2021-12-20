using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RangosDeRedondeoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RangosDeRedondeoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<RangosDeRedondeo, RangosDeRedondeoDto>()
                  .ForMember(x => x.MaterialPorCentroDescripcion, mat => mat.MapFrom(matPorCentro => matPorCentro.MaterialPorCentro.Material.Descripcion));

            Mapper.CreateMap<RangosDeRedondeoDto, RangosDeRedondeo>();
        }
    }
}
