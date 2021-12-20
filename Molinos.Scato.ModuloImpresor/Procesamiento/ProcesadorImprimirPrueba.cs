using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirPrueba : ProcesadorComando<ImprimirPrueba>
   {
        public ProcesadorImprimirPrueba(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirPrueba comando)
        {
            try
            {
                DocumentoImpresion impresora;

                Log.Debug("Iniciando impresión de prueba en la impresora: " + comando.NombreImpresora);
                if (comando.IsZebra)
                {
                    impresora = new PaginaPruebaZebra(comando.NombreUsuario, comando.NombreServidor, comando.NombreImpresora);
                }
                else
                {
                    impresora = new PaginaPrueba(comando.NombreUsuario, comando.NombreServidor, comando.NombreImpresora);
                }


                // printeri
                impresora.Print();

            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.NombreImpresora);
                throw;
            }
            return new Resultado();

        }
    }
}
