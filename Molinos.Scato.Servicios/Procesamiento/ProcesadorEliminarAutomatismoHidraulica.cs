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
    public class ProcesadorEliminarAutomatismoHidraulica : ProcesadorComando<EliminarAutomatismoHidraulicas>
    {
        public ProcesadorEliminarAutomatismoHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarAutomatismoHidraulicas comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorEliminarAutomatismoHidraulica con AutomatismoId = {0}", comando.IdAutomatismo);

                var includes = new List<Expression<Func<AutomatismoGrano, object>>> { x => x.Material, x => x.CallePreBalanza, x => x.CallePreHidraulica, x => x.TipoVariedad, x => x.Almacen, x => x.Hidraulicas };
                var automatismoGranos = Repositorio.Obtener<AutomatismoGrano>(includes, c => c.Id == comando.IdAutomatismo);

                automatismoGranos.Hidraulicas.Clear();

                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorEliminarAutomatismoHidraulica con AutomatismoId = {0}", comando.IdAutomatismo);
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}