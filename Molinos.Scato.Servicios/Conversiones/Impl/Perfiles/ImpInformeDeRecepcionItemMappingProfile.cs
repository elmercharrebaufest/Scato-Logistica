using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class ImpInformeDeRecepcionItemMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpInformeDeRecepcionItemMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpInformeDeRecepcionItem, ImpInformeDeRecepcionItemDto>();
            Mapper.CreateMap<ImpInformeDeRecepcionItemDto, ImpInformeDeRecepcionItem>();
        }
    }
}