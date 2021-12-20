using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CargaComercialMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CargaComercialMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CargaComercial, CargaComercialDto>();
            Mapper.CreateMap<CargaComercialDto, CargaComercial>();
        }
    }
}
