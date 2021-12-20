using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaHabilitacionDeTanquesHistoricoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaHabilitacionDeTanquesHistoricoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaHabilitacionDeTanquesHistorico, ModuloDeCargaHabilitacionDeTanquesHistoricoDto>();
            Mapper.CreateMap<ModuloDeCargaHabilitacionDeTanquesHistoricoDto, ModuloDeCargaHabilitacionDeTanquesHistorico>();
        }
    }
}