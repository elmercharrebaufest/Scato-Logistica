using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorLibroMovimientosExistenciaDeGranosImpresion : ProcesadorComando<LibroMovimientosExistenciaGranosImpresion>
    {
        public ProcesadorLibroMovimientosExistenciaDeGranosImpresion(ILogger log) 
            : base(log)
        {
        }

        public override Resultado Ejecutar(LibroMovimientosExistenciaGranosImpresion command)
        {
            var resultado = new Resultado();

            var impresora = new LibroMovimientosExistenciaGranos(new ImpLibroMovimientosExistenciaGranosDto { Dtos = command.Dtos, Impresora = command.Impresora, EsPdf = false}, command.FormatoDeImpresion);
            Log.Debug("Enviando a la impresora");
            var timeOut = Convert.ToInt32(ConfigurationManager.AppSettings["PrinterTimeOut"]);
            var task = new Task(impresora.Print);
            task.Start();
            task.Wait(timeOut);
            if (task.Exception != null)
            {
                Log.Error(task.Exception, "No se pudo conectar con la impresora");
            }
            if (task.Status != TaskStatus.RanToCompletion)
            {
                resultado.Errores.Add("", String.Format(Textos.ImpresoraNoResponde, timeOut / 1000));
                Log.Warn("La impresora no respondio luego de {0} segundos", timeOut / 1000);
            }
            return resultado;
        }
    }
}
