using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AnalisisObligatorioMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AnalisisObligatorioMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<AnalisisObligatorio, AnalisisObligatorioDto>()
                .ForMember(x => x.MaterialDescripcion, cxc => cxc.MapFrom(x => x.Material.Descripcion));
            Mapper.CreateMap<AnalisisObligatorioDto, AnalisisObligatorio>();
        }
    }
}