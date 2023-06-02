using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class EliminarMuestrasFueraDeFecha : Comando
    {
        
        public bool EsPreLote { get; set; }
        public int HrDiaAnterior { get; set; }

    }
}
