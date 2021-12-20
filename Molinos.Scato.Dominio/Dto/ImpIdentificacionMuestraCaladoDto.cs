using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpIdentificacionMuestraCaladoDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Centro { get; set; }
        public string NumeroCartaPorte { get; set; }
        public string PesoNeto { get; set; }
        public string Humedad { get; set; }
        public string Procedencia { get; set; }
        public DateTime FechaCalado { get; set; }
        public string NombreUsuario { get; set; }
        public string NumeroDeOrden { get; set; }
        public string Patente { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
    }
}
