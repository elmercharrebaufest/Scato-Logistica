using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ExcepcionAlControlMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ExcepcionAlControlMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ExcepcionAlControl, ExcepcionAlControlDto>()
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                  .ForMember(t => t.MaterialDesc, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.RazonSocial, f => f.MapFrom(r => r.Transportista.RazonSocial))
                  .ForMember(t => t.TransportistaId, f => f.MapFrom(r => r.Transportista.Id))
                  .ForMember(t => t.CentroId, f => f.MapFrom(r => r.Centro.Id))
                  .ForMember(t => t.CentroNombre, f => f.MapFrom(r => r.Centro.Descripcion))
                  .ForMember(t => t.TipoDestino, f => f.MapFrom(r => r.TipoDestino))
                  .ForMember(t => t.CentroDestinoId, f => f.MapFrom(r => r.CentroDestino.Id))
                  .ForMember(t => t.ClienteDestinoId, f => f.MapFrom(r => r.ClienteDestino.Id))
                  .ForMember(t => t.DestinoNombre, f => f.MapFrom(r => r.CentroDestino != null ? r.CentroDestino.Descripcion : r.ClienteDestino != null ? r.ClienteDestino.Descripcion : ""));
            Mapper.CreateMap<ExcepcionAlControlDto, ExcepcionAlControl>();
        }
    }
}