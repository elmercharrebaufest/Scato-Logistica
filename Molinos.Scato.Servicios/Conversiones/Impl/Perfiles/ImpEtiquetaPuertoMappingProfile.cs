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
    public class ImpEtiquetaPuertoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ImpEtiquetaPuertoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<ImpEtiquetaPuerto, ImpEtiquetaPuertoDto>();
            Mapper.CreateMap<ImpEtiquetaPuertoDto, ImpEtiquetaPuerto>();
        }
    }
}
