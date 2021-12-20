using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CubitacionDeTanquesMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CubitacionDeTanquesMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CubitacionDeTanques, CubitacionDeTanquesDto>();
            Mapper.CreateMap<CubitacionDeTanquesDto, CubitacionDeTanques>();
        }
    }
}