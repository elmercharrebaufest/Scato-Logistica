using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaManosDeEmbarqueDetalleHistoricoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaManosDeEmbarqueDetalleHistoricoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaManosDeEmbarqueDetalleHistorico, ModuloDeCargaManosDeEmbarqueDetalleHistoricoDto>();
            Mapper.CreateMap<ModuloDeCargaManosDeEmbarqueDetalleHistoricoDto, ModuloDeCargaManosDeEmbarqueDetalleHistorico>();
        }
    }
}