using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CallePreBalanzaPlayaInternaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CallePreBalanzaPlayaInternaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CallePreBalanzaPlayaInterna, CallePreBalanzaPlayaInternaDto>().ReverseMap();
        }
    }
}
