using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AjusteDeStockMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AjusteDeStockMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AjusteDeStock, AjusteDeStockDto>()
                .ForMember(x => x.MaterialId, mat => mat.MapFrom(ajuste => ajuste.Material.Id))
                .ForMember(x => x.MaterialDesc, mat => mat.MapFrom(ajuste => ajuste.Material.Descripcion))
                .ForMember(x => x.TipoComprobanteOnccaId, mat => mat.MapFrom(ajuste => ajuste.TipoComprobanteOncca.Id))
                .ForMember(x => x.TipoComprobanteOnccaDesc, mat => mat.MapFrom(ajuste => ajuste.TipoComprobanteOncca.Descripcion));
            Mapper.CreateMap<AjusteDeStockDto, AjusteDeStock>();
        }
    }
}
