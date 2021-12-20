using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirEtiquetaAuditoriaCamara : ProcesadorComando<ImprimirEtiquetaAuditoriaCamara>
    {
        public ProcesadorImprimirEtiquetaAuditoriaCamara(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirEtiquetaAuditoriaCamara comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de IdentificacionEtiquetaAuditoriaCamara en la impresora: " + comando.Dto.Impresora);

                var impresora = new EtiquetaAuditoriaCamara(comando.Dto, comando.Dto.Impresora);

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
