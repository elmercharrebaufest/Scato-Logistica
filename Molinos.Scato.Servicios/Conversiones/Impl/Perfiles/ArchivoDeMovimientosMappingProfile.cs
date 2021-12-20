using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ArchivoDeMovimientosMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ArchivoDeMovimientosMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ArchivoDeMovimientos, ArchivoDeMovimientosDto>()
                .ForMember(t => t.MaterialDesc, f => f.MapFrom(r => r.Material.Descripcion));
            Mapper.CreateMap<ArchivoDeMovimientosDto, ArchivoDeMovimientos>();
        }
    }
}