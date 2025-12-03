using AutoMapper;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PagoTasaMunicipalMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PagoTasaMunicipalMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<VerificarPagoTasaMunicipal, DatosTasaMunicipal>()
                  .ForMember(x => x.TipoMaterial, mat => mat.MapFrom(tasaMunicipal => ClasificarTipoMaterial(tasaMunicipal)))
                  .ForMember(x => x.AplicaActualizacionInterna, mat => mat.MapFrom(tasaMunicipal => AplicaActualizacionInterna(tasaMunicipal)));

            Mapper.CreateMap<VerificarPagoTasaMunicipal, DatosExcepcionTasaMunicipal>()
                .ForMember(x => x.TipoMaterial, mat => mat.MapFrom(tasaMunicipal => ClasificarTipoMaterial(tasaMunicipal)))
                .ForMember(x => x.EsValidacionAlIngreso, mat => mat.MapFrom(tasaMunicipal => EsValidacionAlIngreso(tasaMunicipal)));
        }

        private TipoMaterial ClasificarTipoMaterial(VerificarPagoTasaMunicipal comando)
        {
            if (comando.EsNoGranos.HasValue)
            {
                return comando.EsNoGranos.Value ? TipoMaterial.NoGranos : TipoMaterial.Granos;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(comando.Ctg))
                    return TipoMaterial.Granos;
                
                else 
                    return TipoMaterial.NoGranos;                
            }
        }
       
        private bool AplicaActualizacionInterna(VerificarPagoTasaMunicipal comando)
        {
            if(comando.InstanceId.HasValue && comando.InstanceId.Value != Guid.Empty)
            {
                return true;
            }
            return false;
        }

        private bool EsValidacionAlIngreso(VerificarPagoTasaMunicipal comando)
        {
            return !comando.InstanceId.HasValue;
        }
    }
}