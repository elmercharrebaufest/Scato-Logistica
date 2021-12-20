using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoDemora : ProcesadorComando<ModificarRecorridoDemora>
    {
        public ProcesadorModificarRecorridoDemora(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoDemora comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                if (comando.DemoraVehiculo.HasValue)
                {
                    recorridoAEditar.VehiculoDemorado = comando.DemoraVehiculo.Value;
                }
                if (comando.DemoraEstablecimiento.HasValue)
                {
                    recorridoAEditar.EstablecimientoDemorado = comando.DemoraEstablecimiento.Value;
                }
                if (!string.IsNullOrEmpty(comando.Motivo))
                {
                recorridoAEditar.MotivoDemora = string.IsNullOrEmpty(recorridoAEditar.MotivoDemora) ? comando.Motivo :
                    recorridoAEditar.MotivoDemora + ", " + comando.Motivo;
                }
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al crear el calado vacío para {0}", comando.InstanceId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
