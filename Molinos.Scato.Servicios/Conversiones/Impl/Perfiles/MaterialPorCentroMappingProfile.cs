using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MaterialPorCentroMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MaterialPorCentroMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MaterialPorCentro, MaterialPorCentroDto>()
                  .ForMember(x => x.AlmacenPredId,mat => mat.MapFrom(matPorCentro => matPorCentro.AlmacenPredeterminado.Id))
                  .ForMember(x => x.AlmacenPredDesc,mat => mat.MapFrom(matPorCentro => matPorCentro.AlmacenPredeterminado.Descripcion))
                  .ForMember(x => x.MaterialId, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.Id))
                  .ForMember(x => x.MaterialDesc, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.Descripcion))
                  .ForMember(x => x.CentroId, mat => mat.MapFrom(matPorCentro => matPorCentro.Centro.Id))
                  .ForMember(x => x.CamaraId, mat => mat.MapFrom(matPorCentro => matPorCentro.Camara.Id))
                  .ForMember(x => x.CamaraDesc, mat => mat.MapFrom(matPorCentro => matPorCentro.Camara.Descripcion))
                  .ForMember(x => x.RequiereTecnologia, mat => mat.MapFrom(matPorCentro => matPorCentro.RequiereTecnologia))
                  .ForMember(x => x.CorrespondeDescarga, mat => mat.MapFrom(matPorCentro => matPorCentro.CorrespondeDescarga))
                  .ForMember(x => x.ImprimeReciboMunicipal, mat => mat.MapFrom(matPorCentro => matPorCentro.ImprimeReciboMunicipal));
            Mapper.CreateMap<MaterialPorCentroDto, MaterialPorCentro>();

            Mapper.CreateMap<MaterialPorCentro, MaterialDto>()
                  .ForMember(x => x.Activo, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.Activo))
                  .ForMember(x => x.AlmacenOrigenDesc, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.AlmacenOrigen.Descripcion))
                  .ForMember(x => x.AlmacenOrigenId, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.AlmacenOrigen.Id))
                  .ForMember(x => x.CodigoEspecie, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.CodigoEspecie))
                  .ForMember(x => x.CodigoONCCA, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.CodigoONCCA))
                  .ForMember(x => x.CodigoSAP, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.CodigoSAP))
                  .ForMember(x => x.Commodity, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.Commodity))
                  .ForMember(x => x.Contrato, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.Contrato))
                  //.ForMember(x => x.Cosecha, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.Cosecha))
                  .ForMember(x => x.Descripcion, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.Descripcion))
                  .ForMember(x => x.DescripcionCorta, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.DescripcionCorta))
                  .ForMember(x => x.EsCosecha, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.EsCosecha))
                  .ForMember(x => x.EsUva, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.EsUva))
                  .ForMember(x => x.FactorConversion, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.FactorConversion))
                  .ForMember(x => x.Id, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.Id))
                  .ForMember(x => x.Lote, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.Lote))
                  .ForMember(x => x.Peso, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.Peso))
                  .ForMember(x => x.RequiereAnexoInase, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.RequiereAnexoInase))
                  .ForMember(x => x.RequiereNumeroTropa, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.RequiereNumeroTropa))
                  .ForMember(x => x.TipoDeGrano, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.TipoDeGrano))
                  .ForMember(x => x.UnidadDeMedidad, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.UnidadDeMedidad))
                  .ForMember(x => x.UsaBinPallet, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.UsaBinPallet))
                  .ForMember(x => x.VariedadId, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.Variedad.Id))
                  .ForMember(x => x.VigenciaDesde, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.VigenciaDesde))
                  .ForMember(x => x.VigenciaHasta, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.VigenciaHasta))
                  .ForMember(x => x.CamaraDesc, mat => mat.MapFrom(matPorCentro => matPorCentro.Camara.Descripcion))
                  .ForMember(x => x.NirsCodigoProducto, mat => mat.MapFrom(matPorCentro => matPorCentro.Material.NirsCodigoProducto))
            ;
            Mapper.CreateMap<MaterialDto, MaterialPorCentro>();
        }
    }
}
