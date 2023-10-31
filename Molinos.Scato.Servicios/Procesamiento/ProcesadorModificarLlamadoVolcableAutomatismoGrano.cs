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
    public class ProcesadorModificarLlamadoVolcableAutomatismoGrano : ProcesadorModificar<ModificarLlamadoVolcableAutomatismoGrano>
    {
        public ProcesadorModificarLlamadoVolcableAutomatismoGrano(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarLlamadoVolcableAutomatismoGrano comando)
        {
            var automatismo = Repositorio.Obtener<AutomatismoGrano>(comando.Id);
            automatismo.Activo = comando.EsLLamadoVolcable;
        }

        protected override void Validar(ModificarLlamadoVolcableAutomatismoGrano comando, Resultado resultado)
        {
            if (comando.EsLLamadoVolcable)
            {
                var includes = new List<Expression<Func<AutomatismoGrano, object>>> { x => x.Material, x => x.CallePreBalanza, x => x.CallePreHidraulica, x => x.TipoVariedad, x => x.Almacen, x => x.Hidraulicas };
                var automatismo = Repositorio.Obtener<AutomatismoGrano>(includes, a => a.Id == comando.Id);
                if(!automatismo.CallePreBalanza.ActivoAutomatico || !automatismo.CallePreHidraulica.ActivoAutomatico || !automatismo.Hidraulicas.Any(x => x.ActivoAutomatico))
                {
                    resultado.Error("Automatismo_CallesDesactivadas", Textos.Automatismo_CallesDesactivadas);
                }
            }
        }
    }
}