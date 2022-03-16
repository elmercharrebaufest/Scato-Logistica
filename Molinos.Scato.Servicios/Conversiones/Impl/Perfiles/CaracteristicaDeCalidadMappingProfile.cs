using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CaracteristicaDeCalidadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CaracteristicaDeCalidadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CaracteristicaDeCalidad, CaracteristicaDeCalidadDto>()
                    .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.MaterialPorCentro.Material.Id))
                    .ForMember(t => t.MaterialDescripcion, f => f.MapFrom(r => r.MaterialPorCentro.Material.Descripcion))
                    .ForMember(t => t.Descripcion, f => f.MapFrom(r => r.CaracteristicaDeCalidadMaestro.Descripcion))
                    .ForMember(t => t.TipoCaracteristica, f => f.MapFrom(r => r.EsHumedad ? CaracteristicasCalidad.EsHumedad : r.EsGranosDañados ? CaracteristicasCalidad.EsGranosDañados : r.EsTenorAzucarino ? CaracteristicasCalidad.EsTenorAzucarino : r.EsEstadoSanitario ? CaracteristicasCalidad.EsEstadoSanitario : r.EsCalidadUva ? CaracteristicasCalidad.EsCalidadUva : r.EsCuerposExtranos ? CaracteristicasCalidad.EsCuerposExtranos : r.EsGranosVerdes ? CaracteristicasCalidad.EsGranosVerdes : r.EsMermaVolatil ? CaracteristicasCalidad.EsMermaVolatil : r.EsProteina ? CaracteristicasCalidad.EsProteina : r.EsInsectosVivos ? CaracteristicasCalidad.EsInsectosVivos : r.EsPesoHectolitrico ? CaracteristicasCalidad.EsPesoHectolitrico: CaracteristicasCalidad.Ninguno));
            Mapper.CreateMap<CaracteristicaDeCalidadDto, CaracteristicaDeCalidad>();
        }
    }
}