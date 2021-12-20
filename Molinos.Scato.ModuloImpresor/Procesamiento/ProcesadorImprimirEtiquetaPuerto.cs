using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Molinos.Scato.ModuloImpresor.Zebra;
using Ninject.Extensions.Logging;
using PrintDoc2Pdf;
using System;
using System.Configuration;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirEtiquetaPuerto : ProcesadorComando<ImprimirEtiquetaPuerto>
    {

        public ProcesadorImprimirEtiquetaPuerto(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirEtiquetaPuerto comando)
        {
            var resultado = new Resultado();

            try
            {
                Log.Debug("Iniciando impresión de ImprimirEtiquetaPuerto en la impresora: " + comando.Impresora);

                var impresora = new EtiquetaPuerto(comando.Dto, comando.Impresora, 0, 0, null);

                if (comando.Impresora == "")
                {
                    try
                    {
                        var resultadoPdf = new ResultadoPrevisualizar();
                        var printer = new pdfPrinter();

                        if (!string.IsNullOrEmpty(impresora.ZplCode))
                        {
                            Log.Debug($"Se va a renderizar contra la impresora {ConfigurationManager.AppSettings["ZebraPrinterIp"]}, el ticket: {impresora.ZplCode}");
                            var imagen = ZebraPrinter.ObtenerImagen(impresora.ZplCode, ConfigurationManager.AppSettings["ZebraPrinterIp"], Log);
                            printer.Document = new ImpresorDeImagenes(imagen);
                        }
                        else
                        {
                            printer.Document = impresora;
                        }
                        printer.Print();
                        resultadoPdf.Archivo = printer.File;

                        return resultadoPdf;
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error al generar etiqueta");
                    }
                }

                impresora.Print();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Impresora);
                throw;
            }
            return resultado;

        }
    }
}
