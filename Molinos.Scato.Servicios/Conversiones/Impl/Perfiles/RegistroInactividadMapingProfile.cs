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
    public class RegistroInactividadMapingProfile: Profile
    {
        public override string ProfileName
        {
            get { return "RegistroInactividadMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<RegistroInactividad, RegistroInactividadDto>();
            Mapper.CreateMap<RegistroInactividadDto, RegistroInactividad>();
        }
    }
}
