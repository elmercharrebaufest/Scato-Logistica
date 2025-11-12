using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogExceptuadosTicketMunicipal: IIdentificable
    {
        public int Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaExcepcion { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public string NombreUsuario { get; set; }
        public string Patente { get; set; }
        public Chofer Chofer { get; set; }
        public Material Material { get; set; }
        public bool PagaTicketMunicipal { get; set; }
    }
}
