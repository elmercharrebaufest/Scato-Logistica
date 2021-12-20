using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ModuloDeCargaManosDeEmbarqueDetalleMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ModuloDeCargaManosDeEmbarqueDetalleMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ModuloDeCargaManosDeEmbarqueDetalle, ModuloDeCargaManosDeEmbarqueDetalleDto>();
            Mapper.CreateMap<ModuloDeCargaManosDeEmbarqueDetalleDto, ModuloDeCargaManosDeEmbarqueDetalle>();
        }
    }
}