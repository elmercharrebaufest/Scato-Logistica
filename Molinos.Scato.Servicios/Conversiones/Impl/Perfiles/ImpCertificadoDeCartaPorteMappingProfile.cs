using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpCertificadoDeCartaPorteMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpCertificadoDeCartaPorteMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpCertificadoDeCartaPorte, ImpCertificadoDeCartaPorteDto>();
            Mapper.CreateMap<ImpCertificadoDeCartaPorteDto, ImpCertificadoDeCartaPorte>();
        }
    }
}
