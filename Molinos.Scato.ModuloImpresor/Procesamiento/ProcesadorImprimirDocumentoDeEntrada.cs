using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirDocumentoDeEntrada : ProcesadorComando<ImprimirDocumentoDeEntrada>
    {


        public ProcesadorImprimirDocumentoDeEntrada(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirDocumentoDeEntrada comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de DocumentoDeEntrada en la impresora: " + comando.Dto.Impresora);


                var impresora = new DocumentoDeEntrada(comando.Dto, comando.Dto.Impresora, comando.Firma);

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
