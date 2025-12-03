using System;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarExcepcionPagoTasaMunicipal : Comando
    { 
        public int Id { get; set; }

        public string Patente { get; set; }
        
        public Guid? WorkflowInstanceId { get; set; }
    }
}
