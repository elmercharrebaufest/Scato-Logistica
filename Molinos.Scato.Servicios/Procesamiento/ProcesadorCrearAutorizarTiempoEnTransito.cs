using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAutorizarTiempoEnTransito : ProcesadorCrear<CrearAutorizarTiempoEnTransito, AutorizarTiempoEnTransito>
    {
        public ProcesadorCrearAutorizarTiempoEnTransito(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override AutorizarTiempoEnTransito CrearEntidad(CrearAutorizarTiempoEnTransito comando)
        {
            var control = Conversor.Convertir<AutorizacionTiempoEnTransitoDto, AutorizarTiempoEnTransito>(comando.Dto);
            control.Fecha = DateTime.Now;
            return control;
        }

        protected override void Validar(CrearAutorizarTiempoEnTransito comando, Resultado resultado)
        {
        }
    }
}
