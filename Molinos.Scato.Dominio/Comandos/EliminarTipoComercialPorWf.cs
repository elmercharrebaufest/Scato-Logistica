using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class EliminarTipoComercialPorWf : Comando
    {
        public int WorkflowId { get; set; }
        public int TipoComercialId { get; set; }
    }
}
