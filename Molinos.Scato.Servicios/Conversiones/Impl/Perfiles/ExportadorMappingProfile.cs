using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ExportadorMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VaporMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Exportador, ExportadorDto>()
                 .ForMember(t => t.AlmacenDesc, f => f.MapFrom(r => r.Almacen.Descripcion))
                 .ForMember(t => t.Almacen_Id, f => f.MapFrom(r => r.Almacen.Id));
            Mapper.CreateMap<ExportadorDto, Exportador>();
        }
    }
}