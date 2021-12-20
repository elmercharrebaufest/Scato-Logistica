using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpCertificadoDeAnalisisMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpCertificadoDeAnalisisMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpCertificadoDeAnalisis, ImpCertificadoDeAnalisisDto>();
            Mapper.CreateMap<ImpCertificadoDeAnalisisDto, ImpCertificadoDeAnalisis>();
        }
    }
}
