using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class EmpresaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "EmpresaMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Empresa, EmpresaDto>();
            Mapper.CreateMap<EmpresaDto, Empresa>();
        }
    }
}