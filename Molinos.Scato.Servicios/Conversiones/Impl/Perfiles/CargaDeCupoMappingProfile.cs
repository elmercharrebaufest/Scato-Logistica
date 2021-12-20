using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System;
using System.Globalization;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class CargaDeCupoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "CargaDeCupoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<CargaDeCupo, CargaDeCupoDto>()
                .ForMember(t => t.FechaSap, f => f.MapFrom(r => r.FechaSap.ToString("yyyy-MM-dd")))
                .ForMember(t => t.CircuitoNoGranos, f => f.MapFrom(r => !r.Material.EsGrano));
            Mapper.CreateMap<CargaDeCupoDto, CargaDeCupo>()
                .ForMember(t => t.FechaSap, f => f.MapFrom(r => string.IsNullOrEmpty(r.FechaSap) ? r.Fecha : DateTime.ParseExact(r.FechaSap, "yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }
    }
}