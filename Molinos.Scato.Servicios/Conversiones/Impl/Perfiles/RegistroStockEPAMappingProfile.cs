using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class RegistroStockEPAMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "RegistroStockEPAMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<RegistroStockEPA, RegistroStockEPADto>()
                .ForMember(x => x.RecorridoId, mat => mat.MapFrom(s => s.Recorrido.Id))
                .ForMember(x => x.InstanceId, mat => mat.MapFrom(s => s.Recorrido.InstanciaWorkflow));
            Mapper.CreateMap<RegistroStockEPADto, RegistroStockEPA>();
        }
    }
}