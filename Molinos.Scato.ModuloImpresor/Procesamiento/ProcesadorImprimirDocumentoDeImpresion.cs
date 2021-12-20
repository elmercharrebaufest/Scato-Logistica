using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirDocumentoDeImpresion : ProcesadorComando<ImprimirDocumentoDeImpresion>
    {
        public ProcesadorImprimirDocumentoDeImpresion(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirDocumentoDeImpresion comando)
        {
            if (comando.FormatoDeImpresion != null)
            {
                try
                {
                    Log.Debug("Iniciando impresión de DocumentoDeImpresion en la impresora: " +
                              comando.Dto.Impresora);


                    var impresora = new ImpresionGenerica(comando.Dto, comando.FormatoDeImpresion);

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
            }
            else
            {
                Log.Error("Error al imprimir en la impresora: codigo de impresion no encontrado");
            }
            return new Resultado();

        }
    }
}
