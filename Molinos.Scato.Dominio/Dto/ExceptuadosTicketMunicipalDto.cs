using Molinos.Scato.Dominio.Recursos;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public class ExceptuadosTicketMunicipalDto
    {
        public int Id { get; set; }
        public string Patente { get; set; }
        public string NombreUsuario { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public string WorkflowCodigo { get; set; }
        public string WorkflowDescripcion { get; set; }
        public DateTime FechaCreacionExcepcion { get; set; }
        public bool Activo { get; set; }
        public bool PermiteAcciones { get; set; }
    }
}
