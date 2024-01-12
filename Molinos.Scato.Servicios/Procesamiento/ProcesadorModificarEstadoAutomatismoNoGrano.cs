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
    public class ProcesadorModificarEstadoAutomatismoNoGrano : ProcesadorModificar<ModificarEstadoAutomatismoNoGrano>
    {
        public ProcesadorModificarEstadoAutomatismoNoGrano(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEstadoAutomatismoNoGrano comando)
        {
            var automatismo = Repositorio.Obtener<AutomatismoNoGrano>(comando.Id);
            automatismo.Activo = comando.Estado;
        }

        protected override void Validar(ModificarEstadoAutomatismoNoGrano comando, Resultado resultado)
        {
            if (comando.Estado)
            {
                var automatismo = Repositorio.Obtener<AutomatismoNoGrano>(comando.Id);
                if (!automatismo.CallePlanta.ActivoAutomatico || !(bool)automatismo.PuntoDeCarga.EstadoAutomatismo || !(bool)automatismo.Almacen.EstadoAutomatismo)
                {
                    resultado.Error("Automatismo_CallesDesactivadas", Textos.Automatismo_CallesDesactivadas);
                }
            }
        }
    }
}