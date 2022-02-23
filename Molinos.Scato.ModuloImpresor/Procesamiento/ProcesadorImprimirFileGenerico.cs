using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirFileGenerico : ProcesadorComando<ImprimirFileGenerico>
    {
        public ProcesadorImprimirFileGenerico(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirFileGenerico comando)
        {
            var resultado = new Resultado();

            try
            {
                if (!string.IsNullOrEmpty(comando.Impresora))
                {
                    try
                    {
                        Log.Debug($"Iniciando impresión de {comando?.CodigoDocumentoImpresion} en la impresora: " +
                                  comando.Impresora);

                        var impresora = new ImpresionFileGenerica(comando);

                        for (var copia = 0; copia < comando.CantidadCopias; copia++)
                        {
                            impresora.Print();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error al imprimir " + comando?.CodigoDocumentoImpresion + " en la impresora: " + comando?.Impresora);
                        throw;
                    }
                }
                else
                {
                    Log.Error("Error al imprimir en la impresora: codigo de impresion no encontrado");
                }
            }
            catch (Exception e)
            {

                throw;
            }

            return resultado;
        }
    }
}