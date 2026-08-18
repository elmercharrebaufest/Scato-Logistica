using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.ColasFIFO.Interfaces;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorDesencolarIdentificacionVehicular : ProcesadorComando<DesencolarIdentificacionVehicular>
    {
        private readonly IColaIdentificacionVehicular cola;

        public ProcesadorDesencolarIdentificacionVehicular(IRepositorio repositorio, IConversor conversor, ILogger log, IColaIdentificacionVehicular cola)
            : base(repositorio, conversor, log)
        {
            this.cola = cola;
        }

        public override Resultado Ejecutar(DesencolarIdentificacionVehicular comando)
        {
            var resultado = new ResultadoEncolarIdentificacionVehicular();

            try
            {
                ColaIdentificacionVehicularDto siguiente = null;

                if (comando.Eliminar)
                    cola.Desencolar(comando.PuestoDeTrabajoId);

                siguiente = cola.ObtenerPrimero(comando.PuestoDeTrabajoId);

                resultado.PrimerElementoCola = siguiente;
            }
            catch (Exception ex)
            {
                Log.Error("Error al desencolar identificación vehicular", ex);
                resultado.Error(nameof(Exception), ex.Message);
            }

            return resultado;
        }
    }
}
