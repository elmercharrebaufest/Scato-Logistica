using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpIdentificacionMuestraAuditoriaDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Centro { get; set; }
        public string NroMuestra { get; set; }
        public string Material { get; set; }
        public int MaterialId { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string NumeroDeOrden { get; set; }
        public string ProveedorCuit { get; set; }
        public string Proveedor { get; set; }
        public string Entregador { get; set; }
        public string EntregadorCuit { get; set; }
        public string CorredorCuit { get; set; }
        public string Corredor { get; set; }
        public string Procedencia { get; set; }
        public string FechaHoraCalado { get; set; }
        public string UsuarioCalado { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
    }
}
