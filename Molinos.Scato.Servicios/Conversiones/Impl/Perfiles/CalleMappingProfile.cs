using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CalleMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CalleMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Calle, CalleDto>()
                .ForMember(t => t.MaterialDesc, f => f.MapFrom(r => r.Material.Descripcion))
                .ForMember(t => t.CaracteristicaDeCalidadDesc, f => f.MapFrom(r => r.CaracteristicaDeCalidad.DescripcionCorta));
            

            Mapper.CreateMap<CalleDto, Calle>();
        }
    }
}