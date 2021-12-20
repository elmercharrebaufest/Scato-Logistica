using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class BodegaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "BodegaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Bodega, BodegaDto>();
            Mapper.CreateMap<BodegaDto, Bodega>();
        }
    }
}