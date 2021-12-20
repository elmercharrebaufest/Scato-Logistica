using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CallePorRecorridoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CallePorRecorridoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CallePorRecorrido, CallePorRecorridoDto>()
                  .ForMember(t => t.TipoCalle, f => f.MapFrom(r => r.Calle.TipoCalle))
                  .ForMember(t => t.Patente, f => f.MapFrom(r => r.Recorrido != null ? r.Recorrido.Patente : r.CargaDeCupo != null ? r.CargaDeCupo.Patente : "E" + r.Id))
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Recorrido != null ? r.Recorrido.Material.Id : r.CargaDeCupo != null && r.CargaDeCupo.Material != null ? r.Recorrido.Material.Id : 0))
                  .ForMember(t => t.Calidad, f => f.MapFrom(r => r.Recorrido != null && r.Recorrido.CaracteristicasAnalizadas !=null? r.Recorrido.CaracteristicasAnalizadas.Calidad : Dominio.Enums.TipoCalidad.Desconocida));
            Mapper.CreateMap<CallePorRecorridoDto, CallePorRecorrido>();
        }
    }
}