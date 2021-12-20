using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class HidraulicaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "HidraulicaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<PuestosDeCargaDescarga, PuestosDeCargaDescargaDto>()
                  .ForMember(x => x.PuestoDeTrabajoId, f => f.MapFrom(r => r.PuestoDeTrabajo.Id))
                  .ForMember(x => x.PuestoDeTrabajo, f => f.MapFrom(r => r.PuestoDeTrabajo.NombrePuesto));
            Mapper.CreateMap<PuestosDeCargaDescargaDto, PuestosDeCargaDescarga>();
        }
    }
}