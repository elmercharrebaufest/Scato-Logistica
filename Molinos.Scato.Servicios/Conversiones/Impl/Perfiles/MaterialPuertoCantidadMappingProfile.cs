using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MaterialPuertoCantidadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MaterialPuertoCantidadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MaterialPuertoCantidad, MaterialPuertoCantidadDto>()
                .ForMember(x => x.MaterialId, x => x.MapFrom(y => y.MaterialPuerto.Id))
                .ForMember(x => x.DescripcionCorta, x => x.MapFrom(y => y.MaterialPuerto.DescripcionCorta))
                .ForMember(x => x.EsLiquido, x => x.MapFrom(y => y.MaterialPuerto.EsLiquido))
                .ForMember(x => x.Color, x => x.MapFrom(y => y.MaterialPuerto.Color));
            Mapper.CreateMap<MaterialPuertoCantidadDto, MaterialPuertoCantidad>();
        }
    }
}
