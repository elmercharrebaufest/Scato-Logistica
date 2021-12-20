using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MotivoQuiebreBarreraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MotivoQuiebreBarreraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MotivoQuiebreBarrera, MotivoQuiebreBarreraDto>()
                  .ForMember(x => x.PuestoTrabajoId, f => f.MapFrom(r => r.PuestoTrabajo.Id))
                  .ForMember(x => x.PuestoTrabajo, f => f.MapFrom(r => r.PuestoTrabajo.NombrePuesto))
                  .ForMember(x => x.Transportista, f => f.MapFrom(r => r.Transportista.RazonSocial))
                  .ForMember(x => x.TransportistaId, f => f.MapFrom(r => r.Transportista.Id))
                  .ForMember(x => x.Hora, f => f.MapFrom(r => r.Fecha.ToShortTimeString()));
            Mapper.CreateMap<MotivoQuiebreBarreraDto, MotivoQuiebreBarrera>();
        }
    }
}