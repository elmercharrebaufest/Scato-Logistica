using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirTicketPesadaAduana : ProcesadorComando<ImprimirTicketPesadaAduana>
    {


        public ProcesadorImprimirTicketPesadaAduana(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirTicketPesadaAduana comando)
        {
            try
            {
                if (comando.CantidadCopias > 5)
                {
                    comando.CantidadCopias = 5;
                }



                for (var copia = 0; copia < comando.CantidadCopias; copia++)
                {
                    comando.Dto.Identidad = (IdentidadDeCopia) copia;

                    var impresora = new TicketPesadaAduana(comando.Dto, comando.Dto.Impresora, comando.Firma);
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
