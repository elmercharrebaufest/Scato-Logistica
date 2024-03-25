using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarAutomatismoTipoVariedades : ProcesadorComando<EliminarAutomatismoTipoVariedades>
    {
        public ProcesadorEliminarAutomatismoTipoVariedades(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarAutomatismoTipoVariedades comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorEliminarAutomatismoTipoVariedades con AutomatismoId = {0}", comando.IdAutomatismo);

                var includes = new List<Expression<Func<AutomatismoGrano, object>>> { x => x.Material, x => x.CallePreBalanza, x => x.CallePreHidraulica, x => x.Almacen, x => x.Hidraulicas, x => x.TipoVariedades };
                var automatismoGranos = Repositorio.Obtener<AutomatismoGrano>(includes, c => c.Id == comando.IdAutomatismo);

                automatismoGranos.TipoVariedades.Clear();

                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorEliminarAutomatismoTipoVariedades con AutomatismoId = {0}", comando.IdAutomatismo);
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}