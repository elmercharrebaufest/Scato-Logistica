using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AutomatismoGranoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AutomatismoGranoMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<AutomatismoGrano, AutomatismoGranoDto>()
                .ForMember(x => x.MaterialDescripcion, mat => mat.MapFrom(m => m.Material.Descripcion))
                .ForMember(x => x.VariedadDescripcion, mat => mat.MapFrom(m => m.TipoVariedad.Descripcion))
                .ForMember(x => x.CallePBDescripcion, mat => mat.MapFrom(m => m.CallePreBalanza.Nombre))
                .ForMember(x => x.CallePHDescripcion, mat => mat.MapFrom(m => m.CallePreHidraulica.Nombre))
                .ForMember(x => x.CallePHTipoLlamadoDescripcion, mat => mat.MapFrom(m => m.CallePreHidraulica.AutomatismoTipoLlamado == null ? string.Empty : m.CallePreHidraulica.AutomatismoTipoLlamado.Descripcion))
                .ForMember(x => x.HidraulicaDescripcion, mat => mat.MapFrom(m => m.Hidraulicas == null ? string.Empty : string.Join(",", m.Hidraulicas.Select(s => s.Nombre))))
                .ForMember(x => x.AlmacenDescripcion, mat => mat.MapFrom(m => m.Almacen.Descripcion))
                .ForMember(x => x.EstadoCallePreBalanza, mat => mat.MapFrom(m => m.CallePreBalanza.Deshabilitada))
                .ForMember(x => x.EstadoCallePreHidraulica, mat => mat.MapFrom(m => m.CallePreHidraulica.Deshabilitada))
                .ForMember(x => x.TipoCallePrebalanza, mat => mat.MapFrom(m => m.CallePreHidraulica.TipoCalle))
                .ForMember(x => x.TipoCallePreHidraulica, mat => mat.MapFrom(m => m.CallePreHidraulica.TipoCalle))
                .ForMember(x => x.Hidraulicas, mat => mat.MapFrom(m => m.Hidraulicas.Select(s => s.Id)))
                .ForMember(x => x.CodigoAutomatismoTipoLlamado, mat => mat.MapFrom(m => m.CallePreHidraulica.AutomatismoTipoLlamado.Codigo));

            Mapper.CreateMap<AutomatismoGranoDto, AutomatismoGrano>()
                .ForMember(x => x.Hidraulicas, mat => mat.Ignore())
                .ForMember(x => x.Almacen, mat => mat.Ignore())
                .ForMember(x => x.Calidad, mat => mat.Ignore())
                .ForMember(x => x.Material, mat => mat.Ignore())
                .ForMember(x => x.TipoVariedad, mat => mat.Ignore())
                .ForMember(x => x.CallePreBalanza, mat => mat.Ignore())
                .ForMember(x => x.CallePreHidraulica, mat => mat.Ignore());
        }
    }
}