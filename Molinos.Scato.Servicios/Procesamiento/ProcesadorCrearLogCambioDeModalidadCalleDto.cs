using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLogCambioDeModalidadCalleDto : ProcesadorCrear<CrearLogCambioDeModalidadCalle, LogCambioDeModalidadCalle>
    {
        public ProcesadorCrearLogCambioDeModalidadCalleDto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override LogCambioDeModalidadCalle CrearEntidad(CrearLogCambioDeModalidadCalle comando)
        {
            var log = new LogCambioDeModalidadCalle()
            {
                Motivo = comando.Dto.Motivo,
                Usuario = comando.Dto.Usuario,
                Automatico = comando.Dto.Activado
            };
            log.Fecha = DateTime.Now;
            log.Calle = Repositorio.Obtener<Calle>(comando.Dto.CalleId);
            return log;
        }

        protected override void Validar(CrearLogCambioDeModalidadCalle comando, Resultado resultado)
        {
        }
    }
}
