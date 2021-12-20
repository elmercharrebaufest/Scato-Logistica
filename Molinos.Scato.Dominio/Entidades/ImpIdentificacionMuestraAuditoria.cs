using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpIdentificacionMuestraAuditoria")]
    public class ImpIdentificacionMuestraAuditoria : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string NroMuestra { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string NumeroDeOrden { get; set; }
        public virtual string Material { get; set; }
        public virtual int MaterialId { get; set; }
        public virtual string ProveedorCuit { get; set; }
        public virtual string EntregadorCuit { get; set; }
        public virtual string CorredorCuit { get; set; }
        public virtual string Procedencia { get; set; }
        public virtual string FechaHoraCalado { get; set; }
        public virtual string UsuarioCalado { get; set; }
        public virtual string Proveedor { get; set; }
        public virtual string Entregador { get; set; }
        public virtual string Corredor { get; set; }
    }
}
