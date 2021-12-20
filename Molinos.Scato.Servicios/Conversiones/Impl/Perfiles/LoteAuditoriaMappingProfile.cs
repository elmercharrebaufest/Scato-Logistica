using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System.Linq;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LoteAuditoriaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LoteAuditoriaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LoteAuditoria, LoteAuditoriaDto>()
                  .ForMember(t => t.CamaraDesc, f => f.MapFrom(r => r.Camara.Descripcion))
                  .ForMember(t => t.CamaraId, f => f.MapFrom(r => r.Camara.Id))
                  .ForMember(t => t.CamaraEmail, f => f.MapFrom(r => r.Camara.Email))
                  .ForMember(t => t.CamaraFormatoDeArchivo, f => f.MapFrom(r => r.Camara.FormatoDeArchivo));
            Mapper.CreateMap<LoteAuditoria, LoteAuditoriaListaDto>()
                  .ForMember(t => t.CamaraDesc, f => f.MapFrom(r => r.Camara.Descripcion))
                  .ForMember(t => t.CamaraId, f => f.MapFrom(r => r.Camara.Id))
                  .ForMember(t => t.MaterialDesc, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.CamaraFormatoDeArchivo, f => f.MapFrom(r => r.Camara.FormatoDeArchivo))
                   .ForMember(t => t.MuestrasSinValorCamara, f => f.MapFrom(r => r.Muestras.Count(y => !y.ValorCamara.HasValue)))
                   .ForMember(t => t.Diferencias, f => f.MapFrom(r => r.Muestras.Count(y => y.Recorrido.CaracteristicasAnalizadasList != null && y.Recorrido.CaracteristicasAnalizadasList.Any() && (y.ValorCamara ?? 0) != (y.Recorrido.CaracteristicasAnalizadasList.First().Grado ?? 0))));
            Mapper.CreateMap<LoteAuditoriaDto, LoteAuditoria>();
        }
    }
}