using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ProveedorExcluidoIntactaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ProveedorExcluidoIntactaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ProveedorExcluidoIntacta, ProveedorExcluidoIntactaDto>()
                  .ForMember(t => t.RazonSocial, f => f.MapFrom(r => r.Proveedor.RazonSocial))
                  .ForMember(t => t.CodigoSap, f => f.MapFrom(r => r.Proveedor.CodigoSap))
                  .ForMember(t => t.Cuil, f => f.MapFrom(r => r.Proveedor.Cuil))
                  .ForMember(t => t.ProveedorId, f => f.MapFrom(r => r.Proveedor.Id));
            Mapper.CreateMap<ProveedorExcluidoIntactaDto, ProveedorExcluidoIntacta>();                
        }
    }
}