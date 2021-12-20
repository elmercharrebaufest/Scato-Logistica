using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios.ServicioImpresion;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorLibroMovimientosExistenciaDeGranosImpresion : ProcesadorImprimirLibroMovimientosExistenciaGranos, IProcesadorComando<LibroMovimientosExistenciaGranosImpresion>
    {
        public ProcesadorLibroMovimientosExistenciaDeGranosImpresion(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresion servicioImpresion) 
            : base(repositorio, conversor, log,firmaProvider, servicioImpresion)
        {
        }

        protected override void GenerarArchivo(ResultadoPrevisualizar resultado, List<ImpImpresionGenericaDto> dtos, FormatoDeImpresionDto formatoDeImpresion, string direccionImpresora)
        {
            ServicioImpresion.Ejecutar(new LibroMovimientosExistenciaGranosImpresion
            {
                Dtos = dtos,
                Impresora = direccionImpresora,
                EsPdf = false,
                FormatoDeImpresion = formatoDeImpresion
            });
        }

        public Resultado Ejecutar(LibroMovimientosExistenciaGranosImpresion comando)
        {
            return base.Ejecutar(comando);
        }
    }
}
