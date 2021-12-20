using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaLineasDeEmbarqueHistoricoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaLineasDeEmbarqueHistoricoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaLineasDeEmbarqueHistorico, ModuloDeCargaLineasDeEmbarqueHistoricoDto>();
            Mapper.CreateMap<ModuloDeCargaLineasDeEmbarqueHistoricoDto, ModuloDeCargaLineasDeEmbarqueHistorico>();
        }
    }
}