using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirEtiquetaAuditoria : ProcesadorComando<ImprimirEtiquetaAuditoria>
    {

        public ProcesadorImprimirEtiquetaAuditoria(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirEtiquetaAuditoria comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirEtiquetaAuditoria en la impresora: " + comando.Dto.Impresora);


                var impresora = new EtiquetaAuditoria(comando.Dto, comando.Dto.Impresora, 0, 0, comando.Firma);
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
