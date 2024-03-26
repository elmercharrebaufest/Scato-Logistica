using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarAutomatismoNoGranosEstado : ProcesadorComando<ActualizarAutomatismoNoGranosEstado>
    {
        public ProcesadorActualizarAutomatismoNoGranosEstado(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarAutomatismoNoGranosEstado comando)
        {
            var resultado = new Resultado();
            var automatismos = Repositorio.Listar<AutomatismoNoGrano>();

            foreach (var automatismo in automatismos)
            {
                automatismo.Activo = comando.Estado;
                automatismo.ActivoLlamado = comando.Estado;
            }

            Repositorio.GuardarCambios();
            return resultado;
        }
    }
}