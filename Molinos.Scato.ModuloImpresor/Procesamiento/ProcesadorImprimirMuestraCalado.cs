using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirMuestraCalado : ProcesadorComando<ImprimirMuestraCalado>
    {
        public ProcesadorImprimirMuestraCalado(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirMuestraCalado comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de IdentificacionMuestraCalado en la impresora: " + comando.Dto.Impresora);
                
                var impresora = new IdentificacionMuestraCalado(comando.Dto, comando.Dto.Impresora);

                for (var copia = 0; copia < comando.CantidadCopias; copia++)
                {
                    impresora.Print();
                }
            }
            catch (Exception e)
            {
                Log.Error(e,"Error al imprimir en la impresora: " + comando.Dto.Impresora);
                throw;
            }
            return new Resultado();

        }
    }
}
