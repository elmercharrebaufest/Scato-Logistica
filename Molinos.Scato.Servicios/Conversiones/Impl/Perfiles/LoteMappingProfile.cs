using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LoteMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LoteMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Lote, LoteDto>()
                  .ForMember(t => t.CamaraDesc, f => f.MapFrom(r => r.Camara.Descripcion))
                  .ForMember(t => t.CamaraId, f => f.MapFrom(r => r.Camara.Id))
                  .ForMember(t => t.CamaraEmail, f => f.MapFrom(r => r.Camara.Email))
                  .ForMember(t => t.CamaraFormatoDeArchivo, f => f.MapFrom(r => r.Camara.FormatoDeArchivo));
            Mapper.CreateMap<Lote, LoteListaDto>()
                  .ForMember(t => t.CamaraDesc, f => f.MapFrom(r => r.Camara.Descripcion))
                  .ForMember(t => t.CamaraId, f => f.MapFrom(r => r.Camara.Id))
                  .ForMember(t => t.CamaraEmail, f => f.MapFrom(r => r.Camara.Email))
                  .ForMember(t => t.CamaraFormatoDeArchivo, f => f.MapFrom(r => r.Camara.FormatoDeArchivo)); 
            Mapper.CreateMap<LoteDto, Lote>();
        }
    }
}