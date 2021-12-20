using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AjusteDeDiferenciasEnRedespachosTransmisionASapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AjusteDeDiferenciasEnRedespachosTransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AjusteDeDiferenciasEnRedespachosTransmisionASap, MovAjuste>()
                .ForMember(x => x.FechaContab, mat => mat.MapFrom(m => m.FechaContab))
                .ForMember(x => x.FechaDoc, mat => mat.MapFrom(m => m.FechaDoc));
            Mapper.CreateMap<MovAjuste, AjusteDeDiferenciasEnRedespachosTransmisionASap>()
                .ForMember(x => x.FechaContab, mat => mat.MapFrom(m => m.FechaContab))
                .ForMember(x => x.FechaDoc, mat => mat.MapFrom(m => m.FechaDoc));
        }
    }
}