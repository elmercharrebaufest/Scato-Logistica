using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PinchazosPorCaladaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PinchazosPorCaladaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<PinchazosPorCalada, PinchazosPorCaladaDto>();
            Mapper.CreateMap<PinchazosPorCaladaDto, PinchazosPorCalada>();
        }
    }
}
