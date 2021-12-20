using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class HistoricoInhabilitacionCamionMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "HistoricoInhabilitacionCamionMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<HistoricoInhabilitacionCamion, HistoricoInhabilitacionCamionDto>();
            Mapper.CreateMap<HistoricoInhabilitacionCamionDto, HistoricoInhabilitacionCamion>();
        }
    }
}