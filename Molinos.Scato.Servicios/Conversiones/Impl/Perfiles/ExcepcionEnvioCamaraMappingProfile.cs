using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ExcepcionEnvioCamaraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ExcepcionEnvioCamaraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ExcepcionEnvioCamara, ExcepcionEnvioCamaraDto>()
                  .ForMember(t => t.MaterialDesc, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.MaterialId, f => f.MapFrom(r => r.Material.Id))
                  .ForMember(t => t.CaracteristicaDesc, f => f.MapFrom(r => r.CaracteristicaMaterial.CaracteristicaDeCalidadMaestro.Descripcion))
                  .ForMember(t => t.CaracteristicaId, f => f.MapFrom(r => r.CaracteristicaMaterial.Id))
                  .ForMember(t => t.TipoComercialDesc, f => f.MapFrom(r => r.TipoComercial.Descripcion))
                  .ForMember(t => t.TipoComercialId, f => f.MapFrom(r => r.TipoComercial.Id))
                  .ForMember(t => t.ProveedorDesc, f => f.MapFrom(r => r.Proveedor.Descripcion))
                  .ForMember(t => t.ProveedorId, f => f.MapFrom(r => r.Proveedor.Id))
                  .ForMember(t => t.EntregadorDesc, f => f.MapFrom(r => r.Entregador.RazonSocial))
                  .ForMember(t => t.EntregadorId, f => f.MapFrom(r => r.Entregador.Id));
            Mapper.CreateMap<ExcepcionEnvioCamaraDto, ExcepcionEnvioCamara>();
        }
    }
}