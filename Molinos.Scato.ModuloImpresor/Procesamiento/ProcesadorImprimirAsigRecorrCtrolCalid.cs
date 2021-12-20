using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirAsigRecorrCtrolCalid : ProcesadorComando<ImprimirAsigRecorrCtrolCalid>
    {


        public ProcesadorImprimirAsigRecorrCtrolCalid(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirAsigRecorrCtrolCalid comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de AsigRecorrCtrolCalid en la impresora: " + comando.Dto.Impresora);

                var impresora = new AsigRecorrCtrolCalid(comando.Dto, comando.Dto.Impresora, comando.Firma);

                for (var copia = 0; copia < comando.CantidadCopias; copia++)
                {
                    impresora.Print();
                }
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
