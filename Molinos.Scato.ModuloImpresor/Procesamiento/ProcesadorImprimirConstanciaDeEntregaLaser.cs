using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirConstanciaDeEntregaLaser : ProcesadorComando<ImprimirConstanciaDeEntregaLaser>
    {


        public ProcesadorImprimirConstanciaDeEntregaLaser(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirConstanciaDeEntregaLaser comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de ConstanciaDeEntregaLaser en la impresora: " + comando.Dto.Impresora);

                var impresora = new ConstanciaDeEntregaLaser(comando.Dto, comando.Dto.Impresora, comando.Firma);

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
