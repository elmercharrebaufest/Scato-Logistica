using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ConversionProcedenciaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ConversionProcedenciaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ConversionProcedencia, ConversionProcedenciaDto>()
                  .ForMember(t => t.ProcedenciaDesc, f => f.MapFrom(r => r.Procedencia.Descripcion))
                  .ForMember(t => t.ProcedenciaId, f => f.MapFrom(r => r.Procedencia.Id))
                  .ForMember(t => t.CamaraDesc, f => f.MapFrom(r => r.Camara.Descripcion))
                  .ForMember(t => t.CamaraId, f => f.MapFrom(r => r.Camara.Id));
            Mapper.CreateMap<ConversionProcedenciaDto, ConversionProcedencia>();
        }
    }
}