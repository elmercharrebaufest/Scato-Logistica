using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirInformeDeRecepcion : ProcesadorComando<ImprimirInformeDeRecepcion>
    {


        public ProcesadorImprimirInformeDeRecepcion(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirInformeDeRecepcion comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de InformeDeRecepcion en la impresora: " + comando.Impresora);

                var impresora = new InformeDeRecepcion(comando.Dto, comando.Impresora, comando.Firma);
                impresora.Print();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Impresora);
                throw;
            }
            return new Resultado();

        }
    }
}
