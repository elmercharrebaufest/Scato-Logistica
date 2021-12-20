using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirSolicitudDeAnalisis : ProcesadorComando<ImprimirSolicitudDeAnalisis>
    {
        public ProcesadorImprimirSolicitudDeAnalisis(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirSolicitudDeAnalisis comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de SolicitudDeAnalisis en la impresora: " + comando.Dto.Impresora);

                var impresora = new SolicitudDeAnalisis(comando.Dto, comando.Dto.Impresora);

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
