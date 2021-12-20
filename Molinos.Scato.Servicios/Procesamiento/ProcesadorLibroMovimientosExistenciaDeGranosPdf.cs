using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios.ServicioImpresion;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorLibroMovimientosExistenciaDeGranosPdf : ProcesadorImprimirLibroMovimientosExistenciaGranos, IProcesadorComando<LibroMovimientosExistenciaGranosPdf>
    {
        public ProcesadorLibroMovimientosExistenciaDeGranosPdf(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresion servicioImpresion)
            : base(repositorio, conversor, log, firmaProvider, servicioImpresion)
        {
        }

        protected override void GenerarArchivo(ResultadoPrevisualizar resultado, List<ImpImpresionGenericaDto> dtos, FormatoDeImpresionDto formatoDeImpresion, string direccionImpresora)
        {
            var r = ServicioImpresion.Ejecutar(new LibroMovimientosExistenciaGranosPdf
            {
                Dtos = dtos,
                Impresora = "PDFPrinter",
                EsPdf = true,
                FormatoDeImpresion = formatoDeImpresion
            }) as ResultadoPrevisualizar;

            if(r != null)
            {
                resultado.Archivo = r.Archivo;
            }
        }

        public Resultado Ejecutar(LibroMovimientosExistenciaGranosPdf comando)
        {
            return base.Ejecutar(comando);
        }
    }
}
