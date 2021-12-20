using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaTabiquesDeEmbarqueHistoricoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaTabiquesDeEmbarqueHistoricoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaTabiquesDeEmbarqueHistorico, ModuloDeCargaTabiquesDeEmbarqueHistoricoDto>();
            Mapper.CreateMap<ModuloDeCargaTabiquesDeEmbarqueHistoricoDto, ModuloDeCargaTabiquesDeEmbarqueHistorico>();
        }
    }
}