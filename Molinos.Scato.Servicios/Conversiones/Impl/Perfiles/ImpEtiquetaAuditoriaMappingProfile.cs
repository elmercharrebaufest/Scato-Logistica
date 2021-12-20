using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpEtiquetaAuditoriaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpEtiquetaAuditoriaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpEtiquetaAuditoria, ImpEtiquetaAuditoriaDto>();
            Mapper.CreateMap<ImpEtiquetaAuditoriaDto, ImpEtiquetaAuditoria>();
        }
    }
}