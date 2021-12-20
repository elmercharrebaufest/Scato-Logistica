using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarDocumento : ProcesadorComando<EliminarDocumento>
    {
        public ProcesadorEliminarDocumento(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarDocumento comando)
        {
            var resultado = new Resultado();
            var id = 0;
            try
            {
                var entidad = Repositorio.Obtener<Dominio.Entidades.Impresion>(comando.Id);
                entidad.Eliminada = true;
                id = entidad.Id;
                Repositorio.GuardarCambios();
            }
            catch (EntidadReferenciadaException)
            {
                resultado.Error("", Textos.Error_EliminarReferenciado);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrio al eliminar la entidad del tipo {0} - Id {1}", typeof(EliminarDocumento).Name, id );
                resultado.Error("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
