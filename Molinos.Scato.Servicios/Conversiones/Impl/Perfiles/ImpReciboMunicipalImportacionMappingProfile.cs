using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpReciboMunicipalImportacionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpReciboMunicipalImportacionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpReciboMunicipalImportacion, ImpReciboMunicipalImportacionDto>();
            Mapper.CreateMap<ImpReciboMunicipalImportacionDto, ImpReciboMunicipalImportacion>();
        }
    }
}
