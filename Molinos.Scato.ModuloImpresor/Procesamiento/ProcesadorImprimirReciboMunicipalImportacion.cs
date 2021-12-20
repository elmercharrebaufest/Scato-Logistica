using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{

    public class ProcesadorImprimirReciboMunicipalImportacion : ProcesadorComando<ImprimirReciboMunicipalImportacion>
    {
        public ProcesadorImprimirReciboMunicipalImportacion(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirReciboMunicipalImportacion comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirReciboMunicipalImportacion en la impresora: " + comando.Dto.Impresora);


                var impresora = new ReciboMunicipalImportacionEtiqueta(comando.Dto, comando.Dto.Impresora, comando.Firma);
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
