using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarMovimientoDeBines:ProcesadorComando<EliminarMovimientoDeBines>
    {
        public ProcesadorEliminarMovimientoDeBines(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarMovimientoDeBines comando)
        {
            var resultado = new Resultado();
            var ajuste = Repositorio.Obtener<MovimientoDeBines>(comando.Id);

            Repositorio.Remover(ajuste);
            try
            {
                Repositorio.GuardarCambios();
            }
            catch (EntidadReferenciadaException)
            {
                Log.Error("Ocurrio al eliminar la entidad del tipo {0} - Id {1}", typeof(MovimientoDeBines).Name, comando.Id);
                resultado.Error("", Textos.Error_ActualizarGenerico);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrio al eliminar la entidad del tipo {0} - Id {1}", typeof(MovimientoDeBines).Name, comando.Id);
                resultado.Error("", Textos.Error_ActualizarGenerico);
            }

            return resultado;
        }
    }
}
