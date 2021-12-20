using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LlegadaAdestinosEnRedespachosTransmisionASapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LlegadaAdestinosEnRedespachosTransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LlegadaAdestinosEnRedespachosTransmisionASap, Mov305>();
            Mapper.CreateMap<Mov305, LlegadaAdestinosEnRedespachosTransmisionASap>();
        }
    }
}