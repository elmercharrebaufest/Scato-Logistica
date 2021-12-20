using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaManosDeEmbarqueHistoricoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaManosDeEmbarqueHistoricoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaManosDeEmbarqueHistorico, ModuloDeCargaManosDeEmbarqueHistoricoDto>()
                .ForMember(x => x.ModuloDeCargaManosDeEmbarqueDetalleHistorico,
                x => x.MapFrom(y => y.ModuloDeCargaManosDeEmbarqueDetalleHistorico));
            Mapper.CreateMap<ModuloDeCargaManosDeEmbarqueHistoricoDto, ModuloDeCargaManosDeEmbarqueHistorico>();
        }
    }
}