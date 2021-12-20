using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpDocumentoDeEntradaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpDocumentoDeEntradaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpDocumentoDeEntrada, ImpDocumentoDeEntradaDto>();
            Mapper.CreateMap<ImpDocumentoDeEntradaDto, ImpDocumentoDeEntrada>();
        }
    }
}
