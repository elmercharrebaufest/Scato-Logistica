using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirMicromuestra : ProcesadorComando<ImprimirMicromuestra>
    {
        public ProcesadorImprimirMicromuestra(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirMicromuestra comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirMicromuestra en la impresora: " + comando.Dto.Impresora);
                var impresora = new IdentificacionMicromuestra(comando.Dto, comando.TipoMicromuestra, comando.Dto.Impresora, 0, 0);

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
