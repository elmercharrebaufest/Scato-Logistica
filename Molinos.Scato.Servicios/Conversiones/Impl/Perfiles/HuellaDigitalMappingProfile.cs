using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using NPOI.POIFS.Properties;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class HuellaDigitalMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "HuellaDigitalMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<HuellaDigital, HuellaDigitalDto>()

                .ForMember(t => t.FechaHoraPesaje, f => f.MapFrom(r => r.FechaHoraPesaje.ToString("yyyy-MM-ddTHH:mm")));


            Mapper.CreateMap<HuellaDigitalDto, HuellaDigital>()
                .ForMember(t => t.FechaHoraPesaje, f => f.MapFrom(r => DateTime.ParseExact(r.FechaHoraPesaje, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture)));



        }

    }
}