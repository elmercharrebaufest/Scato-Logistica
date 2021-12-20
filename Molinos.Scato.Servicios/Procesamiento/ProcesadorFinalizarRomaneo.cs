using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorFinalizarRomaneo : ProcesadorComando<FinalizarRomaneo>
    {
        public ProcesadorFinalizarRomaneo(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(FinalizarRomaneo comando)
        {
            var resultado = new Resultado();
            try
            {
                var romaneos = Repositorio.Listar<Romaneo>(x => x.WorkflowInstanceId == comando.WorkflowId);
                foreach (var romaneo in romaneos)
                {
                    romaneo.Estado = EstadoRomaneo.Finalizado;
                    romaneo.FechaCierre = DateTime.Now;
                }
                if (!resultado.HayErrores)
                {
                    Repositorio.GuardarCambios();
                }
            }
            catch (Exception e)
            {
                Log.Info(e, "Error en la carga de precintos");
                resultado.Error("", Textos.Precinto_ErrorEnLaCarga);
            }
            return resultado;
        }
    }
}
