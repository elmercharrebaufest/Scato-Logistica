using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirGaritaSalida : ProcesadorComando<ImprimirGaritaSalida>
    {


        public ProcesadorImprimirGaritaSalida(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirGaritaSalida comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de GaritaSalida en la impresora: " + comando.Dto.Impresora);

                var impresora = new GaritaSalidaEtiqueta(comando.Dto, comando.Dto.Impresora, comando.Firma);

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
