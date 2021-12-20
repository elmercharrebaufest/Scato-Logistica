using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PesaNetoTransmisionASapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PesaNetoTransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<PesaNetoTransmisionASap, PesaNeto>()
                  .ForMember(x => x.PesoBruto, e => e.MapFrom(m => m.PesoBruto))
                  .ForMember(x => x.PesoBrutoSpecified, e => e.MapFrom(m => true))
                  .ForMember(x => x.PesoNeto, e => e.MapFrom(m => m.PesoNeto))
                  .ForMember(x => x.PesoNetoSpecified, e => e.MapFrom(m => true))
                  .ForMember(x => x.Entrega, e => e.MapFrom(m => m.NumeroDocumento));
            Mapper.CreateMap<PesaNeto, PesaNetoTransmisionASap>()
                .ForMember(x => x.PesoBruto, e => e.MapFrom(m => m.PesoBruto))
                .ForMember(x => x.PesoNeto, e => e.MapFrom(m => m.PesoNeto))
                .ForMember(x => x.NumeroDocumento, e => e.MapFrom(m => m.Entrega));
        }
    }
}