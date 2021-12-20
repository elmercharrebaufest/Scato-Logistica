using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public abstract class ImprimirLibroMovimientosExistenciaGranos : Comando
    {
        public LibroMovimientosExistenciaGranosDto Dto { get; set; }
        public int CentroId { get; set; }
        public string CodigoDeImpresion { get; set; }
    }
}
