using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoAlmacen : ProcesadorComando<ModificarRecorridoAlmacen>
    {
        public ProcesadorModificarRecorridoAlmacen(IRepositorio repositorio, IConversor conversor, ILogger log)
        : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoAlmacen comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                recorridoAEditar.Almacen = Repositorio.Obtener<Almacen>(comando.AlmacenId);
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo actualizar el almacen para el WF: {0}", comando.InstanceId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
