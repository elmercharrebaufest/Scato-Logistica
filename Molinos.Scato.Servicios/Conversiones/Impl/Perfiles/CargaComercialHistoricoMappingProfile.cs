using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CargaComercialHistoricoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CargaComercialHistoricoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CargaComercialHistorico, CargaComercialHistoricoDto>();
            Mapper.CreateMap<CargaComercialHistoricoDto, CargaComercialHistorico>();
        }
    }
}