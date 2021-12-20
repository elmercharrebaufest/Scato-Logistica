using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoTarjetaDeAcceso : ProcesadorComando<ModificarRecorridoTarjetaDeAcceso>
    {
        public ProcesadorModificarRecorridoTarjetaDeAcceso(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }
        public override Resultado Ejecutar(ModificarRecorridoTarjetaDeAcceso comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Debug("Se va a modificar la tarjeta de acceso del recorrido {0} ", comando.InstanceId);
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                recorridoAEditar.TarjetaDeAcceso = comando.Numero;
                Repositorio.GuardarCambios();
                Log.Debug("Tarjeta de acceso del recorrido modificado correctamente {0} ", comando.InstanceId);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al modificar la tarjeta de acceso del recorrido {0}", comando.InstanceId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
