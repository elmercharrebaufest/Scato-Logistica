using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ConversionCaracteristicaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ConversionCaracteristicaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ConversionCaracteristica, ConversionCaracteristicaDto>()
                  .ForMember(t => t.MaterialDesc, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                  .ForMember(t => t.CaracteristicaId, f => f.MapFrom(r => r.Caracteristica.Id))
                  .ForMember(t => t.CaracteristicaDesc, f => f.MapFrom(r => r.Caracteristica.CaracteristicaDeCalidadMaestro.Descripcion))
                  .ForMember(t => t.CamaraDesc, f => f.MapFrom(r => r.Camara.Descripcion))
                  .ForMember(t => t.CamaraId, f => f.MapFrom(r => r.Camara.Id));
            Mapper.CreateMap<ConversionCaracteristicaDto, ConversionCaracteristica>();
        }
    }
}