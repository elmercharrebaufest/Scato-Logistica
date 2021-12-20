using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class BalanzadaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "BalanzadaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Balanzada, BalanzadaDto>()
                .ForMember(x=>x.CargaInicial, f => f.MapFrom(r => r.CargaInicial));
            Mapper.CreateMap<BalanzadaDto, Balanzada>()
                .ForMember(x => x.CargaInicial_Id, f => f.MapFrom(r => r.CargaInicial_Id))
                .ForMember(x => x.CargaInicial_NumeroBalanza, f => f.MapFrom(r => r.NumeroBalanza));
            Mapper.CreateMap<ReportePesadaDto, Balanzada>();
            Mapper.CreateMap<Balanzada, ReportePesadaDto>();
        }
    }
}