using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLogActividad : ProcesadorCrear<CrearLogActividad, LogActividad>
    {
        public ProcesadorCrearLogActividad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override LogActividad CrearEntidad(CrearLogActividad comando)
        {
            var control = Conversor.Convertir<LogActividadDto, LogActividad>(comando.Dto);
            control.Fecha = DateTime.Now;
            return control;
        }

        protected override void Validar(CrearLogActividad comando, Resultado resultado)
        {
        }
    }
}
