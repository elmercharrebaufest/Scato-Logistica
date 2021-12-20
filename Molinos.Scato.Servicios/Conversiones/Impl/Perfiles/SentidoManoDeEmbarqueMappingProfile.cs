using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class SentidoManoDeEmbarqueMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "SentidoManoDeEmbarqueMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<SentidoManoDeEmbarque, SentidoManoDeEmbarqueDto>();
            Mapper.CreateMap<SentidoManoDeEmbarqueDto, SentidoManoDeEmbarque>();
        }
    }
}