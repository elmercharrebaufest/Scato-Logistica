using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpControlDeCargaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpControlDeCargaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpControlDeCarga, ImpControlDeCargaDto>();
            Mapper.CreateMap<ImpControlDeCargaDto, ImpControlDeCarga>();
        }
    }
}
