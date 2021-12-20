using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirControlDeCarga : ProcesadorComando<ImprimirControlDeCarga>
    {
        public ProcesadorImprimirControlDeCarga(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirControlDeCarga comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de ControlDeCarga en la impresora: " + comando.Dto.Impresora);
                var impresora = new ControlDeCarga(comando.Dto, comando.Dto.Impresora)
                    {
                        CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture
                    };
                impresora.Print();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                throw;
            }
            return new Resultado();

        }
    }
}
