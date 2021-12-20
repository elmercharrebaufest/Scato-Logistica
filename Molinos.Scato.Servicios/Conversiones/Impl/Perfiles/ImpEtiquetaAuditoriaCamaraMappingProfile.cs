using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpEtiquetaAuditoriaCamaraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpEtiquetaAuditoriaCamaraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpEtiquetaAuditoriaCamara, ImpEtiquetaAuditoriaCamaraDto>();
            Mapper.CreateMap<ImpEtiquetaAuditoriaCamaraDto, ImpEtiquetaAuditoriaCamara>();
        }
    }
}