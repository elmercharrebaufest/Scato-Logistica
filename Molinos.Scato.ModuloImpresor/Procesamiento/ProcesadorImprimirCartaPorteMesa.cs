using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{

    public class ProcesadorImprimirCartaPorteMesa : ProcesadorComando<ImprimirCartaPorteMesa>
    {


        public ProcesadorImprimirCartaPorteMesa(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirCartaPorteMesa comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirCartaPorteMesa en la impresora: " + comando.Dto.Impresora);


                var impresora = new CartaPorteUrenport(comando.Dto, comando.Dto.Impresora);
                impresora.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;               
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
