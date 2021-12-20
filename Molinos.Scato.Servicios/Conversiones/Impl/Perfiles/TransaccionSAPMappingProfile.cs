using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TransaccionSAPMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TransaccionSAPMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TransaccionSAP, TransaccionSAPDto>()
                  .ForMember(x => x.CentroOrigenId, mat => mat.MapFrom(t => t.CentroOrigen.Id))
                  .ForMember(x => x.CentroOrigenDesc, mat => mat.MapFrom(t => t.CentroOrigen.Descripcion))
                  .ForMember(x => x.MaterialId, mat => mat.MapFrom(t => t.Material.Id))
                  .ForMember(x => x.MaterialDesc, mat => mat.MapFrom(t => t.Material.Descripcion))
                  .ForMember(x => x.MaterialCodigoSAP, mat => mat.MapFrom(t => t.Material.CodigoSAP))
                  .ForMember(x => x.TipoComercialId, mat => mat.MapFrom(t => t.TipoComercial.Id))
                  .ForMember(x => x.TipoComercialDesc, mat => mat.MapFrom(t => t.TipoComercial.Descripcion));
            Mapper.CreateMap<TransaccionSAPDto, TransaccionSAP>();
        }
    }
}
