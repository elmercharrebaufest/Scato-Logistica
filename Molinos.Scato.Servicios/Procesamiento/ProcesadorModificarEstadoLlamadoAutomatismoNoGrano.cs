using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarEstadoLlamadoAutomatismoNoGrano : ProcesadorModificar<ModificarEstadoLlamadoAutomatismoNoGrano>
    {
        public ProcesadorModificarEstadoLlamadoAutomatismoNoGrano(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEstadoLlamadoAutomatismoNoGrano comando)
        {
            var automatismo = Repositorio.Obtener<AutomatismoNoGrano>(comando.Id);
            automatismo.ActivoLlamado = comando.Estado;
        }

        protected override void Validar(ModificarEstadoLlamadoAutomatismoNoGrano comando, Resultado resultado)
        {
        }
    }
}