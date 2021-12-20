using System.Linq;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ConfiguracionDeTablaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ConfiguracionDeTablaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ConfiguracionDeTabla, ConfiguracionDeTablaDto>()
                .ForMember(t => t.MaterialDescc, f => f.MapFrom(r => r.Material.Id))
                .ForMember(t => t.MaterialIdd, f => f.MapFrom(r => r.Material.Id))
                .ForMember(t => t.CaracteristicasDeCalidadDesc, f => f.MapFrom(r => r.CaracteristicasDeCalidad.Select(x => x.CaracteristicaDeCalidadMaestro.Descripcion).ToArray()))
                .ForMember(t => t.CaracteristicasDeCalidadId, f => f.MapFrom(r => r.CaracteristicasDeCalidad.Select(x => x.Id).ToArray()));
            Mapper.CreateMap<ConfiguracionDeTablaDto, ConfiguracionDeTabla>();
        }
    }
}