using System.Linq;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AsignacionDeRecorridoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AsignacionDeRecorridoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AsignacionDeRecorrido, AsignacionDeRecorridoDto>()
                  .ForMember(t => t.Material, f => f.MapFrom(r => r.MaterialPorCentro.Material.Descripcion))
                  .ForMember(t => t.MaterialPorCentroId, f => f.MapFrom(r => r.MaterialPorCentro.Id))
                  .ForMember(t => t.Workflow, f => f.MapFrom(r => r.Workflow.Descripcion))
                  .ForMember(t => t.Calidad, f => f.MapFrom(r => r.Calidad.Descripcion))
                  .ForMember(t => t.CalidadId, f => f.MapFrom(r => r.Calidad.Id))
                  .ForMember(t => t.Calle, f => f.MapFrom(r => r.Calle.Nombre))
                  .ForMember(t => t.CalleId, f => f.MapFrom(r => r.Calle.Id))
                  .ForMember(t => t.BalanzaBruto, f => f.MapFrom(r => r.BalanzaBruto.Nombre))
                  .ForMember(t => t.BalanzaBrutoId, f => f.MapFrom(r => r.BalanzaBruto.Id))
                  .ForMember(t => t.BalanzaTara, f => f.MapFrom(r => r.BalanzaTara.Nombre))
                  .ForMember(t => t.BalanzaTaraId, f => f.MapFrom(r => r.BalanzaTara.Id))
                  .ForMember(t => t.AlmacenDestino, f => f.MapFrom(r => r.AlmacenDestino.Descripcion))
                  .ForMember(t => t.AlmacenDestinoId, f => f.MapFrom(r => r.AlmacenDestino.Id))
                  .ForMember(t => t.HidraulicasId,
                             f => f.MapFrom(r => r.PuestosDeCargaDescargas.Select(x => x.Id).ToArray()));

            Mapper.CreateMap<AsignacionDeRecorridoDto, AsignacionDeRecorrido>();
        }
    }
}