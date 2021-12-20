using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ReglaDeAnalisisObligatorioMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ReglaDeAnalisisObligatorioMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ReglaDeAnalisisObligatorio, ReglaDeAnalisisObligatorioDto>()
                .ForMember(x => x.LocalidadDescripcion, cxc => cxc.MapFrom(x => x.Localidad.Descripcion))
                .ForMember(x => x.ProvinciaDescripcion, cxc => cxc.MapFrom(x => x.Provincia.Descripcion));
            Mapper.CreateMap<ReglaDeAnalisisObligatorioDto, ReglaDeAnalisisObligatorio>();
        }
    }
}