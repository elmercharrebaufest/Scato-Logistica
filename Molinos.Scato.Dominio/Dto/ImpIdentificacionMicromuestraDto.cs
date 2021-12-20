using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpIdentificacionMicromuestraDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Centro { get; set; }
        public string Material { get; set; }
        public string NroMuestra { get; set; }
        public string Patente { get; set; }
        public string NumeroDeOrden { get; set; }
        public string Proveedor { get; set; }
        public string Humedad { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
        public string NroCasillero { get; set; }
        public TipoMicromuestra TipoMicromuestra { get; set; }
    }
}
