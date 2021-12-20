using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using PrintDoc2Pdf;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorLibroMovimientosExistenciaDeGranosPdf : ProcesadorComando<LibroMovimientosExistenciaGranosPdf>
    {
        public ProcesadorLibroMovimientosExistenciaDeGranosPdf(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(LibroMovimientosExistenciaGranosPdf comando)
        {
            var printer = new pdfPrinter();

            var impresora = new LibroMovimientosExistenciaGranos(new ImpLibroMovimientosExistenciaGranosDto { Dtos = comando.Dtos, Impresora = "PDFPrinter", EsPdf = true}, comando.FormatoDeImpresion);
            
            printer.Document = impresora;

            printer.Print();
            return new ResultadoPrevisualizar { Archivo = printer.File };
        }
    }
}
