using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class LoteBiotecnologiaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "LoteBiotecnologiaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<LoteBiotecnologia, LoteBiotecnologiaDto>()
                  .ForMember(t => t.CamaraDesc, f => f.MapFrom(r => r.Camara.Descripcion))
                  .ForMember(t => t.CamaraId, f => f.MapFrom(r => r.Camara.Id))
                  .ForMember(t => t.CamaraEmail, f => f.MapFrom(r => r.Camara.Email))
                  .ForMember(t => t.CamaraFormatoDeArchivo, f => f.MapFrom(r => r.Camara.FormatoDeArchivo));
            Mapper.CreateMap<LoteBiotecnologia, LoteBiotecnologiaListaDto>()
                  .ForMember(t => t.CamaraDesc, f => f.MapFrom(r => r.Camara.Descripcion))
                  .ForMember(t => t.CamaraId, f => f.MapFrom(r => r.Camara.Id))
                  .ForMember(t => t.MaterialDesc, f => f.MapFrom(r => r.Material.Descripcion))
                  .ForMember(t => t.CamaraFormatoDeArchivo, f => f.MapFrom(r => r.Camara.FormatoDeArchivo));
            Mapper.CreateMap<LoteBiotecnologiaDto, LoteBiotecnologia>();
        }
    }
}