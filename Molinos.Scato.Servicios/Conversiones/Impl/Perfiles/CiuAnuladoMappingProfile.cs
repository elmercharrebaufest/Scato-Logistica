using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CiuAnuladoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CiuAnuladoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CiuAnulado, CiuAnuladoDto>();
            Mapper.CreateMap<CiuAnuladoDto, CiuAnulado>();

            Mapper.CreateMap<CiuAnulado, ArchivoINVFilaDto>()
                  .ForMember(t => t.SesentaSiempre, f => f.MapFrom(r => "60"))
                  .ForMember(t => t.NumeroCiu, f => f.MapFrom(r => r.Numero))
                  .ForMember(t => t.FechaEgreso, f => f.MapFrom(r => r.Fecha.ToString("yyyy-MM-dd")))
                  .ForMember(t => t.PesoBruto, f => f.MapFrom(r => "0"))
                  .ForMember(t => t.PesoTaraBodega, f => f.MapFrom(r => "0"))
                  .ForMember(t => t.PesoNeto, f => f.MapFrom(r => "0"))
                  .ForMember(t => t.ModeloCamion, f => f.MapFrom(r => "0"))
                  .ForMember(t => t.NumeroInvVariedadMaterial, f => f.MapFrom(r => "0"))
                  .ForMember(t => t.TenorAzucarino, f => f.MapFrom(r => "0"))
                  .ForMember(t => t.EsPropiaPesTercerosT, f => f.MapFrom(r => "T"))
                  .ForMember(t => t.Campo34, f => f.MapFrom(r => "C"))
                  .ForMember(t => t.CiuCorrectoALRechazadoAN, f => f.MapFrom(r => "AN"))
                  .ForMember(t => t.AñoActual, f => f.MapFrom(r => "0"));
        }

        
    }
}