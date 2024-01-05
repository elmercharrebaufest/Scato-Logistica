using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarAutomatismoGranosEstado : ProcesadorComando<ActualizarAutomatismoGranosEstado>
    {
        public ProcesadorActualizarAutomatismoGranosEstado(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarAutomatismoGranosEstado comando)
        {
            var resultado = new Resultado();
            var automatismos = Repositorio.Listar<AutomatismoGrano>();

            foreach (var automatismo in automatismos)
            {
                automatismo.Activo = comando.Estado;
            }

            Repositorio.GuardarCambios();
            return resultado;
        }
    }
}
