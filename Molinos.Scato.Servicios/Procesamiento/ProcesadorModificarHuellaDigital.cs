using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Reflection;
using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarHuellaDigital : ProcesadorModificar<ModificarHuellaDigital>
    {
        public ProcesadorModificarHuellaDigital(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarHuellaDigital comando)
        {
            var huellaDigital = Repositorio.Obtener<HuellaDigital>(comando.Dto.Id);

            ActualizarDesdeDtoConReflection(huellaDigital, comando.Dto);
        }

        protected override void Validar(ModificarHuellaDigital comando, Resultado resultado)
        {

        }

        private void ActualizarDesdeDtoConReflection(HuellaDigital huella, HuellaDigitalDto dto)
        {
            PropertyInfo[] huellaProps = typeof(HuellaDigital).GetProperties();
            PropertyInfo[] dtoProps = typeof(HuellaDigitalDto).GetProperties();

            foreach (var prop in huellaProps)
            {
                var dtoProp = Array.Find(dtoProps, p => p.Name == prop.Name && p.PropertyType == prop.PropertyType);

                if (dtoProp != null)
                {
                    var huellaValue = prop.GetValue(huella);
                    var dtoValue = dtoProp.GetValue(dto);

                    if (!Equals(huellaValue, dtoValue))
                    {
                        prop.SetValue(huella, dtoValue);
                    }
                }
            }
        }
    }
}
