using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class TecnologiaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "TecnologiaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Tecnologia, TecnologiaDto>()
                .ForMember(t => t.Nombre, f => f.MapFrom(r => r.Nombre))
                .ForMember(t => t.EmpresaNombre, f => f.MapFrom(r => r.Empresa.Nombre))
                .ForMember(t => t.EmpresaId, f => f.MapFrom(r => r.Empresa.Id));
            Mapper.CreateMap<TecnologiaDto, Tecnologia>();
        }
    }
}