using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CategoriaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CategoriaMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<Categoria, CategoriaDto>();
            Mapper.CreateMap<CategoriaDto, Categoria>();
        }
    }
}



