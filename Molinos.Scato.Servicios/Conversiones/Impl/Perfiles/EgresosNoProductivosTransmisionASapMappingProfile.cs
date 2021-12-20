using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EgresosNoProductivosTransmisionASapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EgresosNoProductivosTransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<EgresosNoProductivosTransmisionASap, EgresosNoProductivos>()
                .ForMember(x => x.Fecha, x => x.MapFrom(m => m.FechaOrden))
                .ForMember(x => x.Posiciones, x => x.MapFrom(m =>  new []{new ZSDES9903
                    {
                        DESCRIPCION = m.Descripcion,
                        CENTRO = m.Centro,
                        ALMACEN = m.Almacen,
                        CANTIDAD = m.Cantidad,
                        UNIDAD = m.Unidad,
                        UNIDAD_PESO_ITEM = m.UnidadPesoItem,
                        PESO = m.Peso
                    }}));
            Mapper.CreateMap<EgresosNoProductivos, EgresosNoProductivosTransmisionASap>()
                .ForMember(x => x.FechaOrden, mat => mat.MapFrom(m => m.Fecha))
                .ForMember(x => x.Descripcion, e => e.MapFrom(m => m.Posiciones.Length > 0 ? m.Posiciones[0].DESCRIPCION : ""))
                .ForMember(x => x.Centro, e => e.MapFrom(m => m.Posiciones.Length > 0 ? m.Posiciones[0].CENTRO : ""))
                .ForMember(x => x.Almacen, e => e.MapFrom(m => m.Posiciones.Length > 0 ? m.Posiciones[0].ALMACEN : ""))
                .ForMember(x => x.Cantidad, e => e.MapFrom(m => m.Posiciones.Length > 0 ? m.Posiciones[0].CANTIDAD : 0M))
                .ForMember(x => x.Unidad, e => e.MapFrom(m => m.Posiciones.Length > 0 ? m.Posiciones[0].UNIDAD : ""))
                .ForMember(x => x.UnidadPesoItem, e => e.MapFrom(m => m.Posiciones.Length > 0 ? m.Posiciones[0].UNIDAD_PESO_ITEM : ""))
                .ForMember(x => x.Peso, e => e.MapFrom(m => m.Posiciones.Length > 0 ? m.Posiciones[0].PESO : 0M));
        }
    }
}