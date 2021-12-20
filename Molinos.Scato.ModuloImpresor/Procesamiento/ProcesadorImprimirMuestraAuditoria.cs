using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirMuestraAuditoria : ProcesadorComando<ImprimirMuestraAuditoria>
    {


        public ProcesadorImprimirMuestraAuditoria(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirMuestraAuditoria comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirMuestraAuditoria en la impresora: " + comando.Dto.Impresora);

                var impresora = new IdentificacionMuestraAuditoria(comando.Dto, comando.Dto.Impresora, 0, 0, comando.Firma);

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
