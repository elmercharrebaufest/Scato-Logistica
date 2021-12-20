using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpIdentificacionMuestraAuditoriaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpIdentificacionMuestraAuditoriaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpIdentificacionMuestraAuditoria, ImpIdentificacionMuestraAuditoriaDto>()
                  .ForMember(s => s.MaterialId, t => t.MapFrom(p => p.MaterialId));
            Mapper.CreateMap<ImpIdentificacionMuestraAuditoriaDto, ImpIdentificacionMuestraAuditoria>()
                  .ForMember(s => s.MaterialId, t => t.MapFrom(p => p.MaterialId));
        }
    }
}
