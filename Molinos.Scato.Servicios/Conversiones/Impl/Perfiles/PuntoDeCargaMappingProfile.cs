using System.Linq;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;


namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PuntoDeCargaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PuntoDeCargaMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<PuntoDeCarga, PuntoDeCargaDto>();

            Mapper.CreateMap<PuntoDeCarga, PuntoDeCargaDto>()
                .ForMember(x => x.MaterialesId, mat => mat.MapFrom(m => m.Materiales.Select(y => y.Id)));

            Mapper.CreateMap<PuntoDeCargaDto, PuntoDeCarga>();
        }
    }
}
