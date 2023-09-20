using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class VariedadPorMaterialMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "VariedadPorMaterialMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<TipoVariedadPorMaterial, TipoVariedadPorMaterialDto>()
                .ForMember(v => v.VariedadMaterial, en => en.MapFrom(r => r.TipoVariedad.Descripcion))
                .ForMember(v => v.VariedadMaterialId, en => en.MapFrom(r => r.TipoVariedad.Id));
            Mapper.CreateMap<TipoVariedadPorMaterialDto, TipoVariedadPorMaterial>();
        }
    }
}
