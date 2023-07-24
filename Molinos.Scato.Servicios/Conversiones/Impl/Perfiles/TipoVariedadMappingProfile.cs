using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TipoVariedadMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VariedadMaterialMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<TipoVariedad, TipoVariedadDto>()
                  .ForMember(dto => dto.Id, ent => ent.MapFrom(e => e.Id))
                  .ForMember(dto => dto.Descripcion, ent => ent.MapFrom(e => e.Descripcion))
                  .ForMember(dto => dto.Codigo, ent => ent.MapFrom(e => e.Codigo))
                  .ForMember(dto => dto.Activo, ent => ent.MapFrom(e => e.Activo))
                  .ForMember(dto => dto.FechaCreacion, ent => ent.MapFrom(e => e.FechaCreacion))
                  .ForMember(dto => dto.FechaModificacion, ent => ent.MapFrom(e => e.FechaModificacion))
                  .ForMember(dto => dto.CreadoPor, ent => ent.MapFrom(e => e.CreadoPor))
                  .ForMember(dto => dto.ModificadoPor, ent => ent.MapFrom(e => e.ModificadoPor));

            Mapper.CreateMap<TipoVariedadDto, TipoVariedad>();
        }
    }
}
