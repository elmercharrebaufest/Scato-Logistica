using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TipoComprobanteOnccaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TipoComprobanteOnccaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<TipoComprobanteOncca, TipoComprobanteOnccaDto>();
            Mapper.CreateMap<TipoComprobanteOnccaDto, TipoComprobanteOncca>();
        }
    }
}
