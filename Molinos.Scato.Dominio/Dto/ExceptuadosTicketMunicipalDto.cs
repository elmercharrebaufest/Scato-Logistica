using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ExceptuadosTicketMunicipalDto
    {
        public int Id { get; set; }
        
        public string Patente { get; set; }
        
        public string NombreUsuario { get; set; }
        
        public DateTime FechaCreacionExcepcion { get; set; }

        public Guid? WorkflowInstanceId { get; set; }
    }
}
