using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarExcepcionPagoTasaMunicipal : Comando
    { 
        public int Id { get; set; }
        public string PatenteActual { get; set; }
        public string WorkflowModal { get; set; }
        public string WorkflowDescripcionModal { get; set; }
        public string NumeroDocumentoIngresoActual { get; set; }
        public string NombreUsuario { get; set; }
        public bool TieneRecorrido { get; set; } = false;
    }
}
