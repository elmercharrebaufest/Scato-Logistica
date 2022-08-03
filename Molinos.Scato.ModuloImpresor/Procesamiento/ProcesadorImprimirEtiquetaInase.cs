using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirEtiquetaInase : ProcesadorComando<ImprimirMuestraInase>
    {

        public ProcesadorImprimirEtiquetaInase(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirMuestraInase comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirEtiquetaAuditoria en la impresora: " + comando.Dto.Impresora);


                var impresora = new EtiquetaMuestraInase(comando.Dto, comando.Dto.Impresora);
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
